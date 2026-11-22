using System;
using System.Linq;
using raBudget.Common.Extensions;
using Shouldly;
using Xunit;

namespace raBudget.Common.UnitsTests.DateTimeExtensions
{
    public class EachMonthTo
    {
        [Fact]
        public void EachMonthTo_CorrectlyReturnsPartialMonth()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-10 00:00:00");
            var to = DateTime.Parse("2021-04-13 00:00:00");

            // ACT
            var ranges = from.EachMonthTo(to).ToList();

            // ASSERT
            ranges.ShouldHaveSingleItem();
            ranges.First().Start.ShouldBe(from);
            ranges.First().End.ShouldBe(to);
        }

        [Fact]
        public void EachMonthTo_CorrectlyReturnsFullMonth()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-01 00:00:00");
            var to = DateTime.Parse("2021-05-01 00:00:00");

            // ACT
            var ranges = from.EachMonthTo(to).ToList();

            // ASSERT
            ranges.ShouldHaveSingleItem();
            ranges.First().Start.ShouldBe(from);
            ranges.First().End.ShouldBe(to);
        }

        [Fact]
        public void EachMonthTo_CorrectlyReturnsFullMonthAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-01 00:00:00");
            var to = DateTime.Parse("2021-05-05 00:00:00");

            // ACT
            var ranges = from.EachMonthTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(2);
            ranges.First().Start.ShouldBe(from);
            ranges.First().End.ShouldBe(DateTime.Parse("2021-05-01 00:00:00"));
            
            ranges.Last().Start.ShouldBe(DateTime.Parse("2021-05-01 00:00:00"));
            ranges.Last().End.ShouldBe(to);
        }

        [Fact]
        public void EachMonthTo_CorrectlyReturnsMultipleFullMonthsAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-05 00:30:00");
            var to = DateTime.Parse("2021-07-05 01:00:00");

            // ACT
            var ranges = from.EachMonthTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(4);
            ranges[0].Start.ShouldBe(from);
            ranges[0].End.ShouldBe(DateTime.Parse("2021-05-01 00:00:00"));

            ranges[1].Start.ShouldBe(DateTime.Parse("2021-05-01 00:00:00"));
            ranges[1].End.ShouldBe(DateTime.Parse("2021-06-01 00:00:00"));

            ranges[2].Start.ShouldBe(DateTime.Parse("2021-06-01 00:00:00"));
            ranges[2].End.ShouldBe(DateTime.Parse("2021-07-01 00:00:00"));

            ranges[3].Start.ShouldBe(DateTime.Parse("2021-07-01 00:00:00"));
            ranges[3].End.ShouldBe(to);
        }


    }
}
