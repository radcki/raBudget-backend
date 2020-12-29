using System;

namespace raBudget.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        #region Constructors

        public readonly string Details;
        public NotFoundException(string message)
        {
            Details = message;
        }

        #endregion
    }
}