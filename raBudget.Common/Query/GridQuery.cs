using System.Collections.Generic;

namespace raBudget.Common.Query
{
    public class GridQuery : CollectionQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public List<FieldOrderInfo> DataOrder { get; set; }
    }
}