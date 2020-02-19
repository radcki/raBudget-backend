using raBudget.Domain.BaseTypes;

namespace raBudget.Domain.ValueObjects
{
    public class TransactionId : IdValueBase<int>
    {
        #region Constructors

        public TransactionId(int value) : base(value)
        {
        }

        #endregion
    }
}