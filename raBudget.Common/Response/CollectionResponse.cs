using System.Collections.Generic;

namespace raBudget.Common.Response
{
    public abstract class CollectionResponse<T> : SingleResponse<List<T>>
    {
        public int Count => Data.Count;
        public int Total { get; set; }
    }
}