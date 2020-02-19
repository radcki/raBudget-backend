using System;
using System.Collections.Generic;
using System.Text;
using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    public class BudgetCategoryIcon
    {
        private BudgetCategoryIcon() { }
        public BudgetCategoryIcon(BudgetCategoryIconId iconId, string iconKey)
        {
            IconId = iconId;
            IconKey = iconKey;
        }

        public BudgetCategoryIconId IconId { get; }
        public string IconKey { get; }
    }
}
