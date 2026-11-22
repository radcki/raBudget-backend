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
    public class UpdateBudgetCategoryVisibility
    {
        public class Command : IApplicationRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public bool Hidden { get; set; }
        }

        public class Result: SingleResponse<bool>
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

                budgetCategory.SetVisibility(request.Hidden);

                await _writeDbContext.SaveChangesAsync(cancellationToken);
                
				_ = _eventPublisher.PublishAsync(new Notification()
				{
					ReferenceBudgetCategory = budgetCategory,
				}, cancellationToken);

                return new Result()
                       {
                           Data = budgetCategory.Hidden
                       };
            }
        }
    }
}