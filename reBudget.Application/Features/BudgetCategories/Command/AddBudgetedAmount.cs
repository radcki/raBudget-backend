﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.BudgetCategories.Command
{
    public class AddBudgetedAmount
    {
        public class Command : IRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public decimal Amount { get; set; }
            public DateTime ValidFrom { get; set; }
        }

        public class Result : IdResponse<BudgetedAmountId>
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
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(request.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetCategory = await _writeDbContext.BudgetCategories
                                                          .FirstOrDefaultAsync(x => x.BudgetCategoryId == request.BudgetCategoryId, cancellationToken);
                var budget = _writeDbContext.Budgets.First(x => x.BudgetId == budgetCategory.BudgetId);

                var budgetedAmount = budgetCategory.AddBudgetedAmount(new MoneyAmount(budget.CurrencyCode, request.Amount), request.ValidFrom);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                       {
                           Id = budgetedAmount.BudgetedAmountId
                       };
            }
        }
    }
}