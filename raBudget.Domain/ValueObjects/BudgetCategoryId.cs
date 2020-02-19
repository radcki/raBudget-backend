using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    public class BudgetCategoryId : IdValueBase<int>
    {
        #region Constructors

        public BudgetCategoryId(int value) : base(value)
        {
        }

        #endregion
    }
}