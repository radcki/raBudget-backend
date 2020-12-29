using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace raBudget.Common.Query
{
    [ModelBinder(typeof(GridQueryModelBinder))]
    public class GridQuery : CollectionQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public FieldOrderInfoCollection DataOrder { get; set; }
    }

    //[TypeConverter(typeof(FieldOrderInfoCollectionTypeConverter))]
    public class FieldOrderInfoCollection : List<FieldOrderInfo>
    {
    }

    public class FieldOrderInfoCollectionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return new FieldOrderInfoCollection();
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}