using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;

namespace raBudget.Domain.ReadModels
{
    public class BudgetCategory
    {
        public int BudgetCategoryId { get; set; }
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public eBudgetCategoryType BudgetCategoryType { get; set; }
    }
}
