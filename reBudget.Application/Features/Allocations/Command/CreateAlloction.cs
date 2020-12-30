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

namespace raBudget.Application.Features.Allocations.Command
{
	public class CreateAlloction
	{
		public class Command : IRequest<Result>
		{
			public BudgetCategoryId TargetBudgetCategoryId { get; set; }
			public BudgetCategoryId SourceBudgetCategoryId { get; set; }
			public MoneyAmount Amount { get; set; }
			public string Description { get; set; }
			public DateTime TransactionDate { get; set; }
		}

		public class Result : IdResponse<AllocationId>
		{
		}

		public class Handler : IRequestHandler<Command, Result>
		{
			private readonly IWriteDbContext _writeDbContext;
			private readonly AccessControlService _accessControlService;

			public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService)
			{
				_writeDbContext = writeDbContext;
				_accessControlService = accessControlService;
			}

			public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
			{
				var targetBudgetCategoryId = request.TargetBudgetCategoryId;
				if (!await _accessControlService.HasBudgetCategoryAccessAsync(targetBudgetCategoryId))
				{
					throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetNotFound));
				}

				var sourceBudgetCategoryId = request.SourceBudgetCategoryId;
				if (sourceBudgetCategoryId != null && !await _accessControlService.HasBudgetCategoryAccessAsync(sourceBudgetCategoryId))
				{
					throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetNotFound));
				}

				var targetBudgetCategory = _writeDbContext.BudgetCategories
														  .First(x => x.BudgetCategoryId == request.TargetBudgetCategoryId);
				var sourceBudgetCategory = sourceBudgetCategoryId != null
					? _writeDbContext.BudgetCategories
									 .First(x => x.BudgetCategoryId == request.SourceBudgetCategoryId)
					: null;

				var budget = _writeDbContext.Budgets.First(x => x.BudgetId == targetBudgetCategory.BudgetId);

				var allocation = Allocation.Create(request.Description,
												   targetBudgetCategory,
												   sourceBudgetCategory,
												   request.Amount,
												   request.TransactionDate);
				_writeDbContext.Allocations.Add(allocation);

				await _writeDbContext.SaveChangesAsync(cancellationToken);

				return new Result()
				{
					Id = allocation.AllocationId
				};
			}
		}
	}
}