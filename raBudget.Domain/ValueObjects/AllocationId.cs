using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    public class AllocationId : IdValueBase<int>
    {
        #region Constructors

        public AllocationId(int value) : base(value)
        {
        }

        #endregion
    }
}