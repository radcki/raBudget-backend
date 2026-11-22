using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using raBudget.Application.Infrastructure;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.Transactions.Command
{
    public class UpdateSubTransactionDescription
    {
        public class Command : IApplicationRequest<Result>
        {
            public SubTransactionId SubTransactionId { get; set; }
            public string Description { get; set; }
        }

        public class Result : SingleResponse<string>
        {
        }

        public class Notification : UserNotification
        {
            public Transaction Transaction { get; set; }
            public SubTransaction SubTransaction { get; set; }
        }

        public class Handler : ICommandHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;
            private readonly PostCommitEventPublisher _eventPublisher;

            public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService, PostCommitEventPublisher eventPublisher)
            {
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
                _eventPublisher = eventPublisher;
            }

            public async Task<Result> ExecuteAsync(Command request, CancellationToken cancellationToken)
            {
                var subTransaction = await _writeDbContext.Transactions
                                                          .Include(x => x.SubTransactions)
                                                          .SelectMany(x => x.SubTransactions)
                                                          .FirstOrDefaultAsync(x => x.SubTransactionId == request.SubTransactionId, cancellationToken: cancellationToken)
                                     ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

                if (!await _accessControlService.HasTransactionAccess(subTransaction.ParentTransactionId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }

                subTransaction.SetDescription(request.Description);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                var transaction = _writeDbContext.Transactions.FirstOrDefault(x => x.TransactionId == subTransaction.ParentTransactionId);

                _ = _eventPublisher.PublishAsync(new Notification()
                                      {
                                          Transaction = transaction,
                                          SubTransaction = subTransaction
                                      }, cancellationToken);
                return new Result() {Data = subTransaction.Description};
            }
        }
    }
}