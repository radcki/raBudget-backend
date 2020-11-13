using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace raBudget.Common
{
    public static class KnownCulture
    {
        public const string Polish = "pl-PL";
        public const string English = "en-GB";

        public static IEnumerable<CultureInfo> SupportedCultures()
        {
            foreach (FieldInfo prop in typeof(KnownCulture).GetFields())
            {
                yield return new CultureInfo((string)prop.GetValue(null) ?? string.Empty);
            }
        }
    }
}
