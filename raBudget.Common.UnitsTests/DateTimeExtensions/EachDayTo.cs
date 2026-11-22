using System;
using System.Linq;
using raBudget.Common.Extensions;
using Shouldly;
using Xunit;

namespace raBudget.Common.UnitsTests.DateTimeExtensions
{
    public class EachDayTo
    {
        [Fact]
        public void EachDayTo_CorrectlyReturnsPartialDay()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-08 05:00:00");
            var to = DateTime.Parse("2021-04-09 00:00:00");

            // ACT
            var ranges = from.EachDayTo(to).ToList();

            // ASSERT
            ranges.ShouldHaveSingleItem();
            ranges.First().Start.ShouldBe(from);
            ranges.First().End.ShouldBe(to);
        }

        [Fact]
        public void EachDayTo_CorrectlyReturnsFullDay()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-08 00:00:00");
            var to = DateTime.Parse("2021-04-09 00:00:00");

            // ACT
            var ranges = from.EachDayTo(to).ToList();

            // ASSERT
            ranges.ShouldHaveSingleItem();
            ranges.First().Start.ShouldBe(from);
            ranges.First().End.ShouldBe(to);
        }

        [Fact]
        public void EachDayTo_CorrectlyReturnsFullDayAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-08 00:00:00");
            var to = DateTime.Parse("2021-04-09 01:00:00");

            // ACT
            var ranges = from.EachDayTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(2);
            ranges[0].Start.ShouldBe(from);
            ranges[0].End.ShouldBe(DateTime.Parse("2021-04-09 00:00:00"));

            ranges[0].End.ShouldBe(DateTime.Parse("2021-04-09 00:00:00"));
            ranges[1].End.ShouldBe(to);
        }

        [Fact]
        public void EachDayTo_CorrectlyReturnsMultipleFullDaysAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-08 05:00:00");
            var to = DateTime.Parse("2021-04-10 01:00:00");

            // ACT
            var ranges = from.EachDayTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(3);
            ranges[0].Start.ShouldBe(from);
            ranges[0].End.ShouldBe(DateTime.Parse("2021-04-09 00:00:00"));

            ranges[1].Start.ShouldBe(DateTime.Parse("2021-04-09 00:00:00"));
            ranges[1].End.ShouldBe(DateTime.Parse("2021-04-10 00:00:00"));
            
            ranges[2].Start.ShouldBe(DateTime.Parse("2021-04-10 00:00:00"));
            ranges[2].End.ShouldBe(to);
        }


    }
}
