using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.BaseTypes;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.Models
{
    public class Transaction
    {
        private Transaction()
        {
        }

        public static Transaction Create
        (string description,
         BudgetCategory budgetCategory,
         MoneyAmount amount,
         DateTime transactionDate)
        {
            var transaction = new Transaction
                              {
                                  Description = description
                              };
            transaction.SetBudgetCategory(budgetCategory);
            transaction.SetAmount(amount);
            transaction.SetTransactionDateTime(transactionDate);
            transaction.CreationDateTime = DateTime.Now;

            return transaction;
        }

        public Transaction.Id TransactionId { get; private set; }
        public string Description { get; private set; }
        public BudgetCategory.Id BudgetCategoryId { get; private set; }
        public eBudgetCategoryType TransactionType { get; private set; }
        public MoneyAmount Amount { get; private set; }
        public DateTime TransactionDateTime { get; private set; }
        public DateTime CreationDateTime { get; private set; }

        public void SetAmount(MoneyAmount newAmount)
        {
            if (Amount != null && newAmount.Currency != Amount.Currency)
            {
                throw new BusinessException("New amount must be of same currency");
            }

            Amount = newAmount;
        }

        public void SetTransactionDateTime(DateTime newTransactionDate)
        {
            TransactionDateTime = newTransactionDate.Date;
        }

        public void SetBudgetCategory(BudgetCategory budgetCategory)
        {
            if (TransactionType != default && budgetCategory.BudgetCategoryType != TransactionType)
            {
                throw new BusinessException("New budget category must be of same type as old");
            }

            BudgetCategoryId = budgetCategory.BudgetCategoryId;
            TransactionType = budgetCategory.BudgetCategoryType;
        }

        public class Id : IdValueBase<int>
        {
            public Id(int value) : base(value)
            {
            }
        }
    }
}