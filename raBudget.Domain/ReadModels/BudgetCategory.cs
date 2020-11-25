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
        public string Icon { get; set; }
        public int Order { get; set; }
        public eBudgetCategoryType BudgetCategoryType { get; set; }
    }
}
