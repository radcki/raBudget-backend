using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using raBudget.Common.Extensions;

namespace raBudget.Common
{
    public readonly struct DateRange: IEquatable<DateRange>
    {
        public DateRange(DateTime item1, DateTime item2)
        {
            if (item1 < item2)
            {
                Start = item1;
                End = item2;
            }
            else
            {
                Start = item2;
                End = item1;
            }
        }

        public DateTime Start { get; }
        public DateTime End { get; }

        public bool Contains(DateTime value)
        {
            return value >= Start && value <= End;
        }

        public int DayCount()
        {
            return (int)Math.Ceiling((End - Start).TotalDays);
        }
        public int WeekCount(DayOfWeek startDayOfWeek)
        {
            return Start.EachWeekTo(End, startDayOfWeek).Count();
        }
        public int MonthCount()
        {
            return Start.EachMonthTo(End).Count();
        }

        #region Overrides of ValueType

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Start} - {End}";
        }

        #endregion

        #region Equality members

        /// <inheritdoc />
        public bool Equals(DateRange other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is DateRange other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }

        public static bool operator ==(DateRange left, DateRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DateRange left, DateRange right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}