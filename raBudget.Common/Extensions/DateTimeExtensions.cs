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

        public static DateTime StartOfDay(this DateTime value)
        {
            return value.Date;
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime StartOfMonth(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, 1);
		}

		public static DateTime EndOfMonth(this DateTime value)
		{
			return value.StartOfMonth().AddMonths(1).AddMilliseconds(-1);
        }

        public static int GetQuarterName(this DateTime myDate)
        {
            return (int)Math.Ceiling(myDate.Month / 3.0);
        }

        public static DateTime StartOfQuarter(this DateTime myDate)
        {
            return new DateTime(myDate.Year, (3 * GetQuarterName(myDate)) - 2, 1);
        }

        public static DateTime StartOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1);
        }
        public static IEnumerable<DateRange> EachDayTo(this DateTime from, DateTime to)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }
            DateTime current = from;
            while (current.Date.AddDays(1) < to)
            {
                DateTime next = current.Date.AddDays(1).StartOfDay();
                yield return new DateRange(current, next);
                current = next;
            }
            yield return new DateRange(current, to);
        }

        public static IEnumerable<DateRange> EachMonthTo(this DateTime from, DateTime to)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }
            DateTime current = from;
            while (current.Date.AddMonths(1) < to)
            {
                DateTime next = current.Date.AddMonths(1).StartOfMonth();
                yield return new DateRange(current, next);
                current = next;
            }
            yield return new DateRange(current, to);
        }
        public static IEnumerable<DateRange> EachWeekTo(this DateTime from, DateTime to, DayOfWeek startDayOfWeek)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }
            DateTime current = from;
            while (current.Date.AddDays(7) < to)
            {
                DateTime next = current.Date.AddDays(7).StartOfWeek(startDayOfWeek);
                yield return new DateRange(current, next);
                current = next;
            }
            yield return new DateRange(current, to);
        }
        
        public static IEnumerable<DateRange> EachQuarterTo(this DateTime from, DateTime to)
        { 
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }
            DateTime current = from;
            while (current.Date.AddMonths(3) < to)
            {
                DateTime next = current.Date.AddMonths(3).StartOfQuarter();
                yield return new DateRange(current, next);
                current = next;
            }
            yield return new DateRange(current, to);
        }

        public static IEnumerable<DateRange> EachYearTo(this DateTime from, DateTime to)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }
            DateTime current = from;
            while (current.Date.AddYears(1) < to)
            {
                DateTime next = current.Date.AddYears(1).StartOfYear();
                yield return new DateRange(current, next);
                current = next;
            }
            yield return new DateRange(current, to);
        }
    }

}
