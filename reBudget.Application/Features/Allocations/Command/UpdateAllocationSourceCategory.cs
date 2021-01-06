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

namespace raBudget.Application.Features.Allocations.Command
{
	public class UpdateAllocationSourceCategory
	{
		public class Command : IRequest<Result>
		{
			public AllocationId AllocationId { get; set; }
			public BudgetCategoryId SourceBudgetCategoryId { get; set; }
		}

		public class Result : IdResponse<BudgetCategoryId>
		{
		}

        public class Notification : INotification
        {
            public Allocation Allocation { get; set; }
            public BudgetCategoryId OldBudgetCategoryId { get; set; }
            public BudgetCategoryId NewBudgetCategoryId { get; set; }
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
				if (!await _accessControlService.HasAllocationAccess(request.AllocationId))
				{
					throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
				}

				if (request.SourceBudgetCategoryId != null && !await _accessControlService.HasBudgetCategoryAccessAsync(request.SourceBudgetCategoryId))
				{
					throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
				}

				var allocation = await _writeDbContext.Allocations
													  .FirstAsync(x => x.AllocationId == request.AllocationId, cancellationToken: cancellationToken);

				var oldBudgetCategory = allocation.SourceBudgetCategoryId != null
					? await _writeDbContext.BudgetCategories.FirstAsync(x => x.BudgetCategoryId == allocation.SourceBudgetCategoryId, cancellationToken: cancellationToken)
					: null;

				var newBudgetCategory = request.SourceBudgetCategoryId != null
					? await _writeDbContext.BudgetCategories.FirstAsync(x => x.BudgetCategoryId == request.SourceBudgetCategoryId, cancellationToken: cancellationToken)
					: null;

				if (oldBudgetCategory?.BudgetCategoryType != newBudgetCategory?.BudgetCategoryType)
				{
					throw new BusinessException(Localization.For(() => ErrorMessages.NotSameBudgetCategoryType));
				}

                var oldCategoryId = allocation.SourceBudgetCategoryId;
				allocation.SetSourceBudgetCategory(newBudgetCategory);

				await _writeDbContext.SaveChangesAsync(cancellationToken);

                _ = _mediator.Publish(new Notification()
                                      {
                                          Allocation = allocation,
										  OldBudgetCategoryId = oldCategoryId,
										  NewBudgetCategoryId = allocation.SourceBudgetCategoryId
                                      }, cancellationToken);
				return new Result()
				{
					Id = allocation.SourceBudgetCategoryId
				};
			}
		}
	}
}