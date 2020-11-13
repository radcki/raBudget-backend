﻿using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.Models
{
    public class BudgetCategoryIcon
    {
        private BudgetCategoryIcon() { }
        public BudgetCategoryIcon(BudgetCategoryIcon.Id iconId, string iconKey)
        {
            IconId = iconId;
            IconKey = iconKey;
        }

        public BudgetCategoryIcon.Id IconId { get; }
        public string IconKey { get; }

        public class Id : IdValueBase<int>
        {
            public Id(int value) : base(value)
            {
            }
        }
    }
}