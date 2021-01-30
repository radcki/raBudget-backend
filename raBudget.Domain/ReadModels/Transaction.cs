using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Common.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.ReadModels
{
    public class Transaction
    {
        public TransactionId TransactionId { get; set; }
        public string Description { get; set; }
        public BudgetCategoryId BudgetCategoryId { get; set; }
        public MoneyAmount Amount { get; set; }
        public List<SubTransaction> SubTransactions { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}