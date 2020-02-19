using System;
using System.Collections.Generic;
using System.Text;
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
        public Transaction(TransactionId transactionId, 
                           string description, 
                           BudgetCategory budgetCategory, 
                           MoneyAmount amount, 
                           DateTime transactionDate, 
                           DateTime creationDate)
        {
            TransactionId = transactionId;
            Description = description;
            SetBudgetCategory(budgetCategory);
            SetAmount(amount);
            SetTransactionDateTime(transactionDate);
            CreationDateTime = creationDate;
        }

        public TransactionId TransactionId { get; }
        public string Description { get; private set; }
        public BudgetCategoryId BudgetCategoryId { get; private set; }
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
            TransactionDateTime = newTransactionDate - newTransactionDate.TimeOfDay;
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
    }
}
