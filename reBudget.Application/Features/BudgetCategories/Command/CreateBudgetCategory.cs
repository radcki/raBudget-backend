using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.BudgetCategories.Command
{
    public class CreateBudgetCategory
    {
        public class Command : IRequest<Result>
        {
            public Guid BudgetId { get; set; }
            public string Name { get; set; }
            public Guid BudgetCategoryIconId { get; set; }
            public eBudgetCategoryType BudgetCategoryType { get; set; }
            public List<BudgetedAmount> BudgetedAmounts { get; set; }
        }

        public class BudgetedAmount
        {
            public decimal Amount { get; set; }
            public DateTime ValidFrom { get; set; }
        }

        public class Result
        {
            public Guid BudgetCategoryId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IUserContext _userContext;
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;

            public Handler(IUserContext userContext, IWriteDbContext writeDbContext, AccessControlService accessControlService)
            {
                _userContext = userContext;
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var budgetId = new Domain.Entities.Budget.Id(request.BudgetId);
                if (!await _accessControlService.HasBudgetAccessAsync(budgetId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetNotFound));
                }

                var budget = await _writeDbContext.Budgets
                                                  .FirstOrDefaultAsync(x => x.BudgetId == budgetId
                                                                            && x.OwnerUserId == _userContext.UserId, cancellationToken);

                var icon = await _writeDbContext.BudgetCategoryIcons
                                                .FirstOrDefaultAsync(x => x.IconId.Value == request.BudgetCategoryIconId, cancellationToken);

                var budgetCategory = BudgetCategory.Create(budget, request.Name, icon, request.BudgetCategoryType);
                foreach (var requestBudgetedAmount in request.BudgetedAmounts)
                {
                    budgetCategory.AddBudgetedAmount(new MoneyAmount(budget.Currency, requestBudgetedAmount.Amount), requestBudgetedAmount.ValidFrom);
                }
                _writeDbContext.BudgetCategories.Add(budgetCategory);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                       {
                           BudgetCategoryId = budgetCategory.BudgetCategoryId.Value
                       };
            }
        }
    }
}