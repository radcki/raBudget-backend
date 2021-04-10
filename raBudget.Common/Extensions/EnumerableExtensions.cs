using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raBudget.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static double StandardDeviation<T>(this IEnumerable<T> list, Func<T, double> values)
        {
            var mean = 0.0;
            var sum = 0.0;
            var stdDev = 0.0;
            var n = 0;
            
            foreach (var value in list.Select(values))
            {
                n++;
                var delta = value - mean;
                mean += delta / n;
                sum += delta * (value - mean);
            }
            if (1 < n)
                stdDev = Math.Sqrt(sum / (n - 1));

            return stdDev;

        }
        public static decimal StandardDeviation<T>(this IEnumerable<T> list, Func<T, decimal> values)
        {
            var mean = 0.0m;
            var sum = 0.0m;
            var stdDev = 0.0m;
            var n = 0;

            foreach (var value in list.Select(values))
            {
                n++;
                var delta = value - mean;
                mean += delta / n;
                sum += delta * (value - mean);
            }
            if (1 < n)
                stdDev = (decimal)Math.Sqrt((double)sum / (n - 1));

            return stdDev;

        }
    }
}
