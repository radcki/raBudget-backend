using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    public class BudgetCategoryIconId : IdValueBase<int>
    {
        #region Constructors

        public BudgetCategoryIconId(int value) : base(value)
        {
        }

        #endregion
    }
}