using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;

namespace raBudget.Domain.ReadModels
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int BudgetCategoryId { get; set; }
        public int BudgetId { get; set; }
        public string Description { get; set; }
    }
}
