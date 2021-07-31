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
using raBudget.Common.Query;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.BudgetCategories.Query
{
    public class GetCurrentBudgetCategorySummary
    {
        public class Query : IRequest<Result>
        {
            public List<BudgetCategoryId> BudgetCategoryIds { get; set; }
        }

        public class Result : CollectionResponse<BudgetCategoryBalanceDto>
        {
        }

        public class BudgetCategoryBalanceDto
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public MoneyAmount TotalTransactionsBalance { get; set; }
            public MoneyAmount ThisMonthTransactionsTotal { get; set; }
            public MoneyAmount ThisMonthBudgetedAmount { get; set; }
            public MoneyAmount ThisMonthBudgetedAmountLeft { get; set; }
        }


        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly AccessControlService _accessControlService;
            private readonly BalanceService _balanceService;

            public Handler(AccessControlService accessControlService, BalanceService balanceService)
            {
                _accessControlService = accessControlService;
                _balanceService = balanceService;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var hasBudgetCategoriesAccess = await _accessControlService.HasBudgetCategoriesAccessAsync(request.BudgetCategoryIds);
                if (!hasBudgetCategoriesAccess)
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var responseData = new List<BudgetCategoryBalanceDto>();
                foreach (var budgetCategoryId in request.BudgetCategoryIds)
                {
                    var balances = _balanceService.GetCategoryBalances(budgetCategoryId, null, null).ToList();
                    var thisMonthBalance = balances.FirstOrDefault(x => x.Year == DateTime.Today.Year && x.Month == DateTime.Today.Month);
                    var balance = new BudgetCategoryBalanceDto()
                                  {
                                      BudgetCategoryId = budgetCategoryId,
                                      ThisMonthTransactionsTotal = thisMonthBalance?.TransactionsTotal,
                                      ThisMonthBudgetedAmount = thisMonthBalance?.BudgetedAmount,
                                      TotalTransactionsBalance = thisMonthBalance != null
                                                                     ? new MoneyAmount(thisMonthBalance.BudgetedAmount.CurrencyCode, balances.Sum(x => x.TransactionsTotal.Amount + x.AllocationsTotal.Amount))
                                                                     : null
                                  };
                    if (balance.ThisMonthBudgetedAmount != null)
                    {
                        balance.ThisMonthBudgetedAmountLeft = balance.ThisMonthBudgetedAmount - balance.ThisMonthTransactionsTotal;
                    }

                    responseData.Add(balance);
                }

                return new Result()
                       {
                           Data = responseData,
                           Total = responseData.Count
                       };
            }
        }
    }
}