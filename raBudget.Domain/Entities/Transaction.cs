using System;
using System.Collections.Generic;
using raBudget.Common.Resources;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Domain.Entities
{
    public class Transaction
    {
        private Transaction()
        {
            SubTransactions = new List<SubTransaction>();
        }

        public static Transaction Create
        (string description,
         BudgetCategory budgetCategory,
         MoneyAmount amount,
         DateTime transactionDate)
        {
            var transaction = new Transaction
                              {
                                  TransactionId = new TransactionId(),
                                  Description = description
                              };
            transaction.SetBudgetCategory(budgetCategory);
            transaction.SetAmount(amount);
            transaction.SetTransactionDate(transactionDate);
            transaction.CreationDateTime = DateTime.Now;
            return transaction;
        }

        public TransactionId TransactionId { get; private set; }
        public string Description { get; private set; }
        public BudgetCategoryId BudgetCategoryId { get; private set; }
        public MoneyAmount Amount { get; private set; }
        public List<SubTransaction> SubTransactions { get; set; }
        public DateTime TransactionDate { get; private set; }
        public DateTime CreationDateTime { get; private set; }

        public void SetAmount(MoneyAmount newAmount)
        {
            if (Amount != null && newAmount.Currency != Amount.Currency)
            {
                throw new BusinessException("New amount must be of same currency");
            }

            Amount = newAmount;
        }

        public void SetTransactionDate(DateTime newTransactionDate)
        {
            TransactionDate = newTransactionDate.Date;
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

        public SubTransaction AddSubTransaction
        (string description,
         MoneyAmount amount,
         DateTime transactionDate)
        {
            var subTransaction = SubTransaction.Create(this, description, amount, transactionDate);
            SubTransactions.Add(subTransaction);
            return subTransaction;
        }

        
    }
}