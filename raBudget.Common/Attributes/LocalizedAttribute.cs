using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace raBudget.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class LocalizedAttribute : Attribute
    {
        public CultureInfo Culture { get; }
        public string Text { get; }

        public LocalizedAttribute(string cultureCode, string text)
        {
            this.Culture = new CultureInfo(cultureCode);
            this.Text = text;
        }

        public LocalizedAttribute(CultureInfo culture, string text)
        {
            this.Culture = culture;
            this.Text = text;
        }
    }

    public static class LocalizedExtensions
    {
        public static string ToLocalizedString(this object source)
        {
            return source.ToLocalizedString(CultureInfo.CurrentCulture);
        }

        public static string ToLocalizedString(this object source, CultureInfo culture)
        {
            var attrs = (LocalizedAttribute[]) source.GetType().GetCustomAttributes(typeof(LocalizedAttribute), true);
            return attrs.FirstOrDefault(x => Equals(x.Culture, culture))?.Text;
        }
    }
}