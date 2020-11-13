using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;

namespace raBudget.Domain.ReadModels
{
    public class BudgetCategory
    {
        public Domain.Entities.BudgetCategory.Id BudgetCategoryId { get; set; }
        public Domain.Entities.Budget.Id BudgetId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public eBudgetCategoryType BudgetCategoryType { get; set; }
    }
}
