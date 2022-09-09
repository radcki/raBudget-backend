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
    public class GetBudgetCategoryBalance
    {
        public class Query : GridQuery, IRequest<Result>
        {
            public List<BudgetCategoryId> BudgetCategoryIds { get; set; }
        }

        public class Result : CollectionResponse<BudgetCategoryBalanceDto>
        {
        }

        public class BudgetCategoryBalanceDto
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public MoneyAmount TotalCategoryBalance { get; set; }
            public MoneyAmount ThisMonthTransactionsTotal { get; set; }
            public MoneyAmount BudgetLeftToEndOfYear { get; set; }
            public MoneyAmount ThisYearBudgetedAmount { get; set; }
        }


        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly AccessControlService _accessControlService;
            private readonly BalanceService _balanceService;
            private readonly IReadDbContext _readDbContext;

            public Handler(AccessControlService accessControlService, BalanceService balanceService, IReadDbContext readDbContext)
            {
                _accessControlService = accessControlService;
                _balanceService = balanceService;
                _readDbContext = readDbContext;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var hasBudgetCategoriesAccess = await _accessControlService.HasBudgetCategoriesAccessAsync(request.BudgetCategoryIds);
                if (!hasBudgetCategoriesAccess)
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetCategories = _readDbContext.BudgetCategories.Where(x => request.BudgetCategoryIds.Contains(x.BudgetCategoryId)).ToList();
                var balances = budgetCategories
                              .Select(x => _balanceService.GetTotalCategoryBalance(x))
                              .Select(x => new BudgetCategoryBalanceDto()
                                           {
                                               BudgetCategoryId = x.BudgetCategoryId,
                                               BudgetLeftToEndOfYear = x.BudgetLeftToEndOfYear,
                                               ThisMonthTransactionsTotal = x.ThisMonthTransactionsTotal,
                                               TotalCategoryBalance = x.TotalCategoryBalance,
                                               ThisYearBudgetedAmount = x.ThisYearBudgetedAmount
                                           })
                              .ToList();

                return new Result()
                       {
                           Data = balances,
                           Total = balances.Count
                       };
            }
        }
    }
}