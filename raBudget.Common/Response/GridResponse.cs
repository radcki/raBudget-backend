using System;

namespace raBudget.Common.Response
{
    public abstract class GridResponse<T> : CollectionResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => Math.Max(Total / PageSize, 0);
    }
}