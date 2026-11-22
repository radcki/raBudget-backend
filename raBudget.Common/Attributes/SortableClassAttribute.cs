using System;

namespace raBudget.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SortableClassAttribute : Attribute
    {
        public readonly string SortProperty;
        public SortableClassAttribute(string propertyName)
        {
            SortProperty = propertyName;
        }
    }
}
