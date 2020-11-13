using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;

namespace raBudget.Domain.ReadModels
{
    public class Transaction
    {
        public Domain.Entities.Transaction.Id TransactionId { get; set; }
        public Domain.Entities.BudgetCategory.Id BudgetCategoryId { get; set; }
        public string Description { get; set; }
    }
}
