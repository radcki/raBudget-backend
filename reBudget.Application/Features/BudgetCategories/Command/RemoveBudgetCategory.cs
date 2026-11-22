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

namespace raBudget.Application.Features.BudgetCategories.Command
{
    public class RemoveBudgetCategory
    {
        public class Command : IApplicationRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
        }

        public class Result : BaseResponse
        {
        }
        
		public class Notification : UserNotification
		{
			public BudgetCategory ReferenceBudgetCategory { get; set; }
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
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(request.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetCategory = await _writeDbContext.BudgetCategories
                                                          .FirstOrDefaultAsync(x => x.BudgetCategoryId == request.BudgetCategoryId, cancellationToken);

                await _writeDbContext.Allocations.Where(x => x.SourceBudgetCategoryId == budgetCategory.BudgetCategoryId || x.TargetBudgetCategoryId == budgetCategory.BudgetCategoryId).ForEachAsync(x => x.SoftDelete(), cancellationToken: cancellationToken);
                await _writeDbContext.Transactions.Where(x => x.BudgetCategoryId == budgetCategory.BudgetCategoryId).ForEachAsync(x => x.SoftDelete(), cancellationToken: cancellationToken);

                budgetCategory.SoftDelete();

                await _writeDbContext.SaveChangesAsync(cancellationToken);

				_ = _eventPublisher.PublishAsync(new Notification()
				{
					ReferenceBudgetCategory = budgetCategory,
				}, cancellationToken);

                return new Result()
                       {
                       };
            }
        }
    }
}