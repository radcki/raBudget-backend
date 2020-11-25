using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.Transactions.Command
{
    public class CreateTransaction
    {
        public class Command : IRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public DateTime TransactionDate { get; set; }
        }

        public class Result : IdResponse<TransactionId>
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
                var budgetCategoryId = request.BudgetCategoryId;
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(budgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetNotFound));
                }

                var budgetCategory = _writeDbContext.BudgetCategories
                                                    .First(x => x.BudgetCategoryId == request.BudgetCategoryId);

                var budget = _writeDbContext.Budgets.First(x => x.BudgetId == budgetCategory.BudgetId);

                var transaction = Transaction.Create(request.Description, 
                                                     budgetCategory, 
                                                     new MoneyAmount(budget.Currency, request.Amount), 
                                                     request.TransactionDate);
                _writeDbContext.Transactions.Add(transaction);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                       {
                           Id = transaction.TransactionId
                       };
            }
        }
    }
}