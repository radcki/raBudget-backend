using System;

namespace raBudget.Common.Response
{
    public abstract class BaseResponse
    {
        public DateTime ServerTime => DateTime.Now;
    }
}