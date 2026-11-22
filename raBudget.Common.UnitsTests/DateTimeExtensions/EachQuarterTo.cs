using System;
using System.Linq;
using raBudget.Common.Extensions;
using Shouldly;
using Xunit;

namespace raBudget.Common.UnitsTests.DateTimeExtensions
{
    public class EachQuarterTo
    {
        [Fact]
        public void EachQuarterTo_CorrectlyReturnsPartialQuarter()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-08 00:00:00");
            var to = DateTime.Parse("2021-04-10 00:00:00");

            // ACT
            var ranges = from.EachQuarterTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(1);
            ranges.First().Start.ShouldBe(from);
            ranges.First().End.ShouldBe(to);
        }

        [Fact]
        public void EachQuarterTo_CorrectlyReturnsFullQuarter()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-01 00:00:00");
            var to = DateTime.Parse("2021-07-01 00:00:00");

            // ACT
            var ranges = from.EachQuarterTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(1);
            ranges.First().Start.ShouldBe(from);
            ranges.First().End.ShouldBe(to);
        }

        [Fact]
        public void EachQuarterTo_CorrectlyReturnsFullQuarterAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-01 00:00:00");
            var to = DateTime.Parse("2021-08-15 00:00:00");

            // ACT
            var ranges = from.EachQuarterTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(2);
            ranges[0].Start.ShouldBe(from);
            ranges[0].End.ShouldBe(DateTime.Parse("2021-07-01 00:00:00"));

            ranges[0].End.ShouldBe(DateTime.Parse("2021-07-01 00:00:00"));
            ranges[1].End.ShouldBe(to);
        }

        [Fact]
        public void EachQuarterTo_CorrectlyReturnsMultipleFullQuartersAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-06 07:00:00");
            var to = DateTime.Parse("2021-10-28 00:40:00");

            // ACT
            var ranges = from.EachQuarterTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(3);
            ranges[0].Start.ShouldBe(from);
            ranges[0].End.ShouldBe(DateTime.Parse("2021-07-01 00:00:00"));

            ranges[1].Start.ShouldBe(DateTime.Parse("2021-07-01 00:00:00"));
            ranges[1].End.ShouldBe(DateTime.Parse("2021-10-01 00:00:00"));
            
            ranges[2].Start.ShouldBe(DateTime.Parse("2021-10-01 00:00:00"));
            ranges[2].End.ShouldBe(to);
        }


    }
}
