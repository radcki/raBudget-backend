using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Interfaces;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.BudgetCategories.Query
{
	public class GetBudgetCategoriesBalance
	{
		public class Query : IRequest<Result>
		{
			public BudgetId BudgetId { get; set; }
			public List<eBudgetCategoryType> BudgetCategoryTypes { get; set; }
		}

		public class Result : CollectionResponse<BudgetCategoryBalanceDto>
		{
		}

		public class BudgetCategoryBalanceDto
		{
			public BudgetCategoryId BudgetCategoryId { get; set; }
			public string BudgetCategoryIconKey { get; set; }
			public BudgetCategoryIconId BudgetCategoryIconId { get; set; }
			public BudgetId BudgetId { get; set; }
			public int Order { get; set; }
			public string Name { get; set; }
			public MoneyAmount TotalCategoryBalance { get; set; }
			public MoneyAmount ThisMonthTransactionsTotal { get; set; }
			public MoneyAmount BudgetLeftToEndOfYear { get; set; }
		}


		public class Handler : IRequestHandler<Query, Result>
		{
			private readonly IReadDbContext _readDb;
			private readonly AccessControlService _accessControlService;
			private readonly BalanceService _balanceService;

			public Handler(IReadDbContext readDb, AccessControlService accessControlService, BalanceService balanceService)
			{
				_readDb = readDb;
				_accessControlService = accessControlService;
				_balanceService = balanceService;
			}

			public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
			{
				var budgetCategoryIds = _accessControlService.GetAccessibleBudgetCategoryIds(request.BudgetId);

				var query = _readDb.BudgetCategories
								   .Where(x => budgetCategoryIds.Contains(x.BudgetCategoryId) && x.BudgetId == request.BudgetId);

				if (request.BudgetCategoryTypes != null && request.BudgetCategoryTypes.Any())
				{
					query = query.Where(x => request.BudgetCategoryTypes.Contains(x.BudgetCategoryType));
				}

				var budgetCategories = await query.ToListAsync(cancellationToken: cancellationToken);
				var endDate = new DateTime(DateTime.Today.Year, 12, 1);
				var tasks = budgetCategories.Select(async budgetCategory =>
											{
												var dto = new BudgetCategoryBalanceDto()
												{
													BudgetCategoryId = budgetCategory.BudgetCategoryId,
													Name = budgetCategory.Name,
													BudgetCategoryIconKey = budgetCategory.BudgetCategoryIconKey,
													BudgetId = budgetCategory.BudgetId,
													BudgetCategoryIconId = budgetCategory.BudgetCategoryIconId,
													Order = budgetCategory.Order
												};

												await foreach (var budgetCategoryBalance in _balanceService.GetCategoryBalances(budgetCategory.BudgetCategoryId, null, endDate, cancellationToken))
												{
													if (dto.TotalCategoryBalance == null)
													{
														dto.TotalCategoryBalance = budgetCategoryBalance.BudgetedAmount - budgetCategoryBalance.TransactionsTotal;
													}
													else if (budgetCategoryBalance.Year < DateTime.Today.Year
															 || (budgetCategoryBalance.Year == DateTime.Today.Year && budgetCategoryBalance.Month <= DateTime.Today.Month))
													{
														dto.TotalCategoryBalance += budgetCategoryBalance.BudgetedAmount - budgetCategoryBalance.TransactionsTotal;
													}

													if (dto.BudgetLeftToEndOfYear == null)
													{
														dto.BudgetLeftToEndOfYear = budgetCategoryBalance.BudgetedAmount - budgetCategoryBalance.TransactionsTotal;
													}
													else
													{
														dto.BudgetLeftToEndOfYear += budgetCategoryBalance.BudgetedAmount - budgetCategoryBalance.TransactionsTotal;
													}

													if (budgetCategoryBalance.Month == DateTime.Today.Month && budgetCategoryBalance.Year == DateTime.Today.Year)
													{
														dto.ThisMonthTransactionsTotal = budgetCategoryBalance.TransactionsTotal;
													}
												}

												return dto;
											})
											.ToList();
				var data = new List<BudgetCategoryBalanceDto>();
				foreach (var task in tasks)
				{
					data.Add(await task);
				}
				return new Result()
				{
					Data = data.OrderBy(x => x.Order).ToList(),
					Total = data.Count()
				};
			}
		}
	}
}