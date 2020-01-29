using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    public class BudgetId : IdValueBase<int>
    {
        #region Constructors

        public BudgetId(int value) : base(value)
        {
        }

        #endregion
    }
}