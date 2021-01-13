using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
