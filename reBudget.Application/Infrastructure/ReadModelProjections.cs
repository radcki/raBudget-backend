using System;
using System.Linq;
using raBudget.Application.Features.Allocations.Query;
using raBudget.Application.Features.BudgetCategories.Query;
using raBudget.Application.Features.Budgets.Query;
using raBudget.Application.Features.Dictionaries.Query;
using raBudget.Application.Features.Transactions.Query;
using raBudget.Application.Features.TransactionTemplates.Query;
using raBudget.Domain.Entities;
using Allocation = raBudget.Domain.ReadModels.Allocation;
using Budget = raBudget.Domain.ReadModels.Budget;
using BudgetCategory = raBudget.Domain.ReadModels.BudgetCategory;
using BudgetCategoryIcon = raBudget.Domain.ReadModels.BudgetCategoryIcon;
using Transaction = raBudget.Domain.ReadModels.Transaction;
using TransactionTemplate = raBudget.Domain.ReadModels.TransactionTemplate;

namespace raBudget.Application.Infrastructure
{
    internal static class ReadModelProjections
    {
        /// <summary>Projects allocations for the allocation grid.</summary>
        public static IQueryable<GetAllocationList.AllocationDto> ToAllocationDtos(this IQueryable<Allocation> allocations)
        {
            return allocations.Select(allocation => new GetAllocationList.AllocationDto
            {
                AllocationId = allocation.AllocationId,
                TargetBudgetCategoryId = allocation.TargetBudgetCategoryId,
                SourceBudgetCategoryId = allocation.SourceBudgetCategoryId,
                Amount = allocation.Amount,
                Description = allocation.Description,
                AllocationDate = allocation.AllocationDate,
                CreationDateTime = allocation.CreationDateTime
            });
        }

        /// <summary>Projects budget details for the budget list.</summary>
        public static IQueryable<GetBudgetList.BudgetDto> ToBudgetDtos(this IQueryable<Budget> budgets)
        {
            return budgets.Select(budget => new GetBudgetList.BudgetDto
            {
                BudgetId = budget.BudgetId,
                StartingDate = budget.StartingDate,
                Currency = budget.Currency,
                Name = budget.Name
            });
        }

        /// <summary>Projects categories and their budgeted amounts in validity order.</summary>
        public static IQueryable<GetBudgetCategoryList.BudgetCategoryDto> ToBudgetCategoryDtos(this IQueryable<BudgetCategory> categories)
        {
            var today = DateTime.Today;
            return categories.Select(category => new GetBudgetCategoryList.BudgetCategoryDto
            {
                BudgetCategoryId = category.BudgetCategoryId,
                BudgetCategoryIconKey = category.BudgetCategoryIconKey,
                BudgetCategoryIconId = category.BudgetCategoryIconId,
                BudgetId = category.BudgetId,
                BudgetCategoryType = category.BudgetCategoryType,
                Order = category.Order,
                Name = category.Name,
                Hidden = category.Hidden,
                CurrentBudgetedAmount = category.BudgetedAmounts
                    .Where(amount => amount.ValidFrom <= today && (amount.ValidTo == null || amount.ValidTo >= today))
                    .Select(amount => amount.Amount)
                    .FirstOrDefault(),
                BudgetedAmounts = category.BudgetedAmounts
                    .OrderBy(amount => amount.ValidFrom)
                    .Select(amount => new GetBudgetCategoryList.BudgetedAmountDto
                    {
                        BudgetedAmountId = amount.BudgetedAmountId,
                        ValidFrom = amount.ValidFrom,
                        ValidTo = amount.ValidTo,
                        Amount = amount.Amount
                    })
                    .ToList()
            });
        }

        /// <summary>Projects the category icon dictionary.</summary>
        public static IQueryable<GetBudgetCategoryIcons.BudgetCategoryIconDto> ToBudgetCategoryIconDtos(this IQueryable<BudgetCategoryIcon> icons)
        {
            return icons.Select(icon => new GetBudgetCategoryIcons.BudgetCategoryIconDto
            {
                BudgetCategoryIconId = icon.BudgetCategoryIconId,
                IconKey = icon.IconKey
            });
        }

        /// <summary>Projects currencies and their culture-derived display metadata.</summary>
        public static IQueryable<GetCurrencies.CurrencyDto> ToCurrencyDtos(this IQueryable<Currency> currencies)
        {
            return currencies.Select(currency => GetCurrencies.CurrencyDto.CreateFrom(currency));
        }

        /// <summary>Projects transactions with their subtransactions ordered by newest date first.</summary>
        public static IQueryable<GetTransactionList.TransactionDto> ToTransactionDtos(this IQueryable<Transaction> transactions)
        {
            return transactions.Select(transaction => new GetTransactionList.TransactionDto
            {
                TransactionId = transaction.TransactionId,
                BudgetCategoryId = transaction.BudgetCategoryId,
                Amount = transaction.Amount,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                CreationDateTime = transaction.CreationDateTime,
                SubTransactions = transaction.SubTransactions
                    .OrderByDescending(subTransaction => subTransaction.TransactionDate)
                    .Select(subTransaction => new GetTransactionList.SubTransactionDto
                    {
                        SubTransactionId = subTransaction.SubTransactionId,
                        Amount = subTransaction.Amount,
                        Description = subTransaction.Description,
                        TransactionDate = subTransaction.TransactionDate,
                        CreationDateTime = subTransaction.CreationDateTime
                    })
                    .ToList()
            });
        }

        /// <summary>Projects the fields shown in the transaction template list.</summary>
        public static IQueryable<GetTransactionTemplateList.TransactionTemplateDto> ToTransactionTemplateDtos(this IQueryable<TransactionTemplate> templates)
        {
            return templates.Select(template => new GetTransactionTemplateList.TransactionTemplateDto
            {
                TransactionTemplateId = template.TransactionTemplateId,
                BudgetCategoryId = template.BudgetCategoryId,
                Amount = template.Amount,
                Description = template.Description
            });
        }
    }
}
