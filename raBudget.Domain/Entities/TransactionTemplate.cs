using System;
using System.Collections.Generic;
using raBudget.Common.Entities;
using raBudget.Common.Resources;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Domain.Entities
{
    public class TransactionTemplate : BaseEntity
    {
        private TransactionTemplate()
        {
        }

        public static TransactionTemplate Create
        (string description,
         BudgetCategory budgetCategory,
         MoneyAmount amount)
        {
            var transaction = new TransactionTemplate
            {
                                  TransactionTemplateId = new TransactionTemplateId(),
                                  Description = description
                              };
            transaction.SetBudgetCategory(budgetCategory);
            transaction.SetAmount(amount);
            transaction.CreationDateTime = DateTime.Now;
            return transaction;
        }

        public TransactionTemplateId TransactionTemplateId { get; private set; }
        public string Description { get; private set; }
        public BudgetCategoryId BudgetCategoryId { get; private set; }
        public MoneyAmount Amount { get; private set; }
        public DateTime CreationDateTime { get; private set; }

        public void SetAmount(MoneyAmount newAmount)
        {
            if (Amount != null && newAmount.Currency != Amount.Currency)
            {
                throw new BusinessException("New amount must be of same currency");
            }

            Amount = newAmount;
        }
        
        public void SetDescription(string description)
        {
            description = description.Trim();
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessException(Localization.For(()=>ErrorMessages.TransactionDescriptionEmpty));
            }

            Description = description;
        }

        public void SetBudgetCategory(BudgetCategory budgetCategory)
        {
            if (budgetCategory == default)
            {
                throw new BusinessException(Localization.For(() => ErrorMessages.BudgetCategoryEmpty));
            }
            BudgetCategoryId = budgetCategory.BudgetCategoryId;
        }
        
    }
}