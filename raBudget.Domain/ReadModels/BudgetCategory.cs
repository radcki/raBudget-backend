using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.ReadModels
{
    public class BudgetCategory
    {
        public BudgetCategoryId BudgetCategoryId { get; set; }
        public BudgetId BudgetId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public string BudgetCategoryIconKey { get; set; }
        public BudgetCategoryIconId BudgetCategoryIconId { get; set; }
        public List<BudgetedAmount> BudgetedAmounts { get; set; }
        public eBudgetCategoryType BudgetCategoryType { get; set; }

        public class BudgetedAmount
        {
            public BudgetedAmountId BudgetedAmountId { get; set; }
            public DateTime ValidFrom { get; set; }
            public DateTime? ValidTo { get; set; }
            public MoneyAmount Amount { get; set; }
        }
    }

}
