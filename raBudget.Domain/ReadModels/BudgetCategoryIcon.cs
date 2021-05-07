using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Enums;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.ReadModels
{
    public class BudgetCategoryIcon
    {
        public BudgetCategoryIconId BudgetCategoryIconId { get; set; }
        public string IconKey { get; set; }
    }
}
