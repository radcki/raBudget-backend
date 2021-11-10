using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.Enums;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.ReadModels
{
    public class Allocation
    {
        public AllocationId AllocationId { get; set; }
        public string Description { get; set; }
        public BudgetCategoryId TargetBudgetCategoryId { get; set; }
        public BudgetCategoryId SourceBudgetCategoryId { get; set; }
        public MoneyAmount Amount { get; set; }
        public DateTime AllocationDate { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}