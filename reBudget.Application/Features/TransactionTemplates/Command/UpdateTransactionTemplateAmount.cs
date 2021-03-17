using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.TransactionTemplates.Command
{
    public class UpdateTransactionTemplateAmount
    {
        public class Command : IRequest<Result>
        {
            public TransactionTemplateId TransactionTemplateId { get; set; }
            public MoneyAmount Amount { get; set; }
        }

        public class Result : SingleResponse<MoneyAmount>
        {
        }

		public class Notification : INotification
		{
			public TransactionTemplate TransactionTemplate { get; set; }
		}

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;
            private readonly IMediator _mediator;

            public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService, IMediator mediator)
            {
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
                _mediator = mediator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                
                if (!await _accessControlService.HasTransactionTemplateAccess(request.TransactionTemplateId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }
                var transactionTemplate = await _writeDbContext.TransactionTemplates
                                                       .FirstOrDefaultAsync(x => x.TransactionTemplateId == request.TransactionTemplateId, cancellationToken: cancellationToken)
                                  ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

                transactionTemplate.SetAmount(request.Amount);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                _ = _mediator.Publish(new Notification()
                                      {
                                          TransactionTemplate = transactionTemplate
                                      }, cancellationToken);

                return new Result()
                       {
                           Data = transactionTemplate.Amount
                       };
            }
        }
    }
}