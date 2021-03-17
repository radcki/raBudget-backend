using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
    public class CreateTransactionTemplate
    {
        public class Command : IRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public MoneyAmount Amount { get; set; }
            public string Description { get; set; }
        }

        public class Result : IdResponse<TransactionTemplateId>
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
                var budgetCategoryId = request.BudgetCategoryId;
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(budgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetCategory = _writeDbContext.BudgetCategories
                                                    .First(x => x.BudgetCategoryId == request.BudgetCategoryId);

                var transactionTemplate = TransactionTemplate.Create(request.Description,
                                                     budgetCategory,
                                                     request.Amount);

                _writeDbContext.TransactionTemplates.Add(transactionTemplate);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                _ = _mediator.Publish(new Notification() {
                                          TransactionTemplate = transactionTemplate
                                      }, cancellationToken);

                return new Result()
                       {
                           Id = transactionTemplate.TransactionTemplateId
                       };
            }
        }
    }
}