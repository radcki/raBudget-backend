using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.Entities
{
    public class BudgetCategoryIcon
    {
        private BudgetCategoryIcon() { }

        public static BudgetCategoryIcon Create(string iconKey)
        {
            return new BudgetCategoryIcon(new BudgetCategoryIconId(), iconKey);
        }
        public BudgetCategoryIcon(BudgetCategoryIconId budgetCategoryIconId, string iconKey)
        {
            BudgetCategoryIconId = budgetCategoryIconId;
            IconKey = iconKey;
        }

        public BudgetCategoryIconId BudgetCategoryIconId { get; private set; }
        public string IconKey { get; private set; }

        
    }
}
