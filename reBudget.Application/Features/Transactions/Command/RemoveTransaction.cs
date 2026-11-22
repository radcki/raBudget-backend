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
    public class RemoveTransaction
    {
        public class Command : IApplicationRequest<Result>
        {
            public TransactionId TransactionId { get; set; }
        }

        public class Result : BaseResponse
        {
        }

        public class Notification : UserNotification
		{
			public Transaction Transaction { get; set; }
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
                var transaction = await _writeDbContext.Transactions
                                                       .FirstOrDefaultAsync(x => x.TransactionId == request.TransactionId, cancellationToken: cancellationToken)
                                  ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

                if (!await _accessControlService.HasTransactionAccess(transaction.TransactionId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }

                transaction.SoftDelete();

                await _writeDbContext.SaveChangesAsync(cancellationToken);


                _ = _eventPublisher.PublishAsync(new Notification()
                                      {
                                          Transaction = transaction
                                      }, cancellationToken);

                return new Result() { };
            }
        }
    }
}