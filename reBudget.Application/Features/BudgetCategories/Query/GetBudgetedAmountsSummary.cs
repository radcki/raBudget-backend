using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using raBudget.Common.Interfaces;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.BudgetCategories.Query
{
    public class GetBudgetedAmountsSummary
    {
        public class Query : IRequest<Result>
        {
            public BudgetId BudgetId { get; set; }
        }

        public class Result : SingleResponse<SummaryDto>
        {
        }

        public class SummaryDto
        {
            public BudgetedAmountSummaryDto SpendingSummary { get; set; }
            public BudgetedAmountSummaryDto IncomeSummary { get; set; }
            public BudgetedAmountSummaryDto SavingSummary { get; set; }
            public BudgetedAmountSummaryDto TotalSummary { get; set; }
        }

        public class BudgetedAmountSummaryDto
        {
            public MoneyAmount CurrentBudgetedAmount { get; set; }
            public MoneyAmount ThisYearBudgetedAmount { get; set; }
            public MoneyAmount TotalBudgetedAmount { get; set; }
        }


        public class Mapper : IHaveCustomMapping
        {
            public void CreateMappings(Profile configuration)
            {
            }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly AccessControlService _accessControlService;
            private readonly BalanceService _balanceService;
            private readonly IReadDbContext _readDb;

            public Handler(AccessControlService accessControlService, BalanceService balanceService, IReadDbContext readDb)
            {
                _accessControlService = accessControlService;
                _balanceService = balanceService;
                _readDb = readDb;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var budgetCategoryIdsQuery = _accessControlService.GetAccessibleBudgetCategoryIds(request.BudgetId);
                var budgetCategories = _readDb.BudgetCategories.Where(x => budgetCategoryIdsQuery.Contains(x.BudgetCategoryId)).ToList();

                var spendingBudgetCategoryIds = budgetCategories.Where(x=>x.BudgetCategoryType == eBudgetCategoryType.Spending).Select(x=>x.BudgetCategoryId).ToList();
                var incomeBudgetCategoryIds = budgetCategories.Where(x=>x.BudgetCategoryType == eBudgetCategoryType.Income).Select(x=>x.BudgetCategoryId).ToList();
                var savingBudgetCategoryIds = budgetCategories.Where(x => x.BudgetCategoryType == eBudgetCategoryType.Saving).Select(x => x.BudgetCategoryId).ToList();

                var balances = budgetCategories
                              .Select(x => _balanceService.GetTotalCategoryBalance(x))
                              .ToList();


                var spendingBalances = balances.Where(x => spendingBudgetCategoryIds.Contains(x.BudgetCategoryId)).ToList();
                var incomeBalances = balances.Where(x => incomeBudgetCategoryIds.Contains(x.BudgetCategoryId)).ToList();
                var savingBalances = balances.Where(x => savingBudgetCategoryIds.Contains(x.BudgetCategoryId)).ToList();


                var spendingSummary = new BudgetedAmountSummaryDto
                                      {
                                          CurrentBudgetedAmount = spendingBalances.Select(x => x.ThisMonthBudgetedAmount).Where(x=>x != null).Aggregate((a, b) => a + b),
                                          TotalBudgetedAmount = spendingBalances.Select(x => x.TotalBudgetedAmount).Where(x => x != null).Aggregate((a, b) => a + b),
                                          ThisYearBudgetedAmount = spendingBalances.Select(x => x.ThisYearBudgetedAmount).Where(x => x != null).Aggregate((a, b) => a + b),
                                      };
                var incomeSummary = new BudgetedAmountSummaryDto
                                    {
                                        CurrentBudgetedAmount = incomeBalances.Select(x => x.ThisMonthBudgetedAmount).Where(x => x != null).Aggregate((a, b) => a + b),
                                        TotalBudgetedAmount = incomeBalances.Select(x => x.TotalBudgetedAmount).Where(x => x != null).Aggregate((a, b) => a + b),
                                        ThisYearBudgetedAmount = incomeBalances.Select(x => x.ThisYearBudgetedAmount).Where(x => x != null).Aggregate((a, b) => a + b),
                                    };
                var savingSummary = new BudgetedAmountSummaryDto
                                    {
                                        CurrentBudgetedAmount = savingBalances.Select(x => x.ThisMonthBudgetedAmount).Where(x => x != null).Aggregate((a, b) => a + b),
                                        TotalBudgetedAmount = savingBalances.Select(x => x.TotalBudgetedAmount).Where(x => x != null).Aggregate((a, b) => a + b),
                                        ThisYearBudgetedAmount = savingBalances.Select(x => x.ThisYearBudgetedAmount).Where(x => x != null).Aggregate((a, b) => a + b),
                                    };

                var totalSummary = new BudgetedAmountSummaryDto
                                   {
                                       CurrentBudgetedAmount = incomeSummary.CurrentBudgetedAmount - spendingSummary.CurrentBudgetedAmount - savingSummary.CurrentBudgetedAmount,
                                       TotalBudgetedAmount = incomeSummary.TotalBudgetedAmount - spendingSummary.TotalBudgetedAmount - savingSummary.TotalBudgetedAmount,
                                       ThisYearBudgetedAmount = incomeSummary.ThisYearBudgetedAmount - spendingSummary.ThisYearBudgetedAmount - savingSummary.ThisYearBudgetedAmount,
                                   };

                return new Result()
                       {
                           Data = new SummaryDto()
                                  {
                                      SpendingSummary = spendingSummary,
                                      IncomeSummary = incomeSummary,
                                      SavingSummary = savingSummary,
                                      TotalSummary = totalSummary
                                  }
                       };
            }
        }
    }
}