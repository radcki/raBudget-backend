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
        public class Query : IRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
        }

        public class Result : SingleResponse<BudgetCategoryBalanceDto>
        {
        }

        public class BudgetCategoryBalanceDto
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
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
                var hasBudgetCategoryAccess = await _accessControlService.HasBudgetCategoryAccessAsync(request.BudgetCategoryId);
                if (!hasBudgetCategoryAccess)
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var endDate = new DateTime(DateTime.Today.Year, 12, 1);
                var dto = new BudgetCategoryBalanceDto()
                          {
                              BudgetCategoryId = request.BudgetCategoryId,
                          };

                foreach (var budgetCategoryBalance in _balanceService.GetCategoryBalances(request.BudgetCategoryId, null, endDate))
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

                return new Result()
                       {
                           Data = dto,
                       };
            }
        }
    }
}