using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raBudget.Common.Extensions
{
    public static class DateTimeExtensions
    {
		public static IEnumerable<DateTime> MonthRange(DateTime from, DateTime to)
		{
			var current = from.StartOfMonth();
			while (current.Date <= to.Date || current.StartOfMonth() == from.StartOfMonth())
			{
				yield return current.StartOfMonth();
				current = current.AddMonths(1);
			}
		}

		public static DateTime StartOfMonth(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, 1);
		}

		public static DateTime EndOfMonth(this DateTime value)
		{
			return value.StartOfMonth().AddMonths(1).AddMilliseconds(-1);
		}
    }

}
