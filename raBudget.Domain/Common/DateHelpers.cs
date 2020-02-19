using System;
using System.Collections.Generic;
using System.Text;

namespace raBudget.Domain.Common
{
    public static class DateHelpers
    {
        public static DateTime FirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime EndOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }
    }
}
