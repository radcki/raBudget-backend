using System;

namespace raBudget.Domain.Exceptions
{
    public class BusinessException : Exception
    {
        #region Constructors
        public readonly string Details;
        public BusinessException(string message)
        {
            Details = message;
        }

        #endregion
    }
}