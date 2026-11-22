using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using raBudget.Application.Infrastructure;
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
        public class Command : IApplicationRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public MoneyAmount Amount { get; set; }
            public string Description { get; set; }
        }

        public class Result : IdResponse<TransactionTemplateId>
        {
        }

		public class Notification : UserNotification
		{
            public TransactionTemplate TransactionTemplate { get; set; }
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

                _ = _eventPublisher.PublishAsync(new Notification() {
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