using System;
using System.Linq;
using raBudget.Common.Extensions;
using Shouldly;
using Xunit;

namespace raBudget.Common.UnitsTests.DateTimeExtensions
{
    public class EachYearTo
    {
        [Fact]
        public void EachYearTo_CorrectlyReturnsPartialYear()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-08 00:00:00");
            var to = DateTime.Parse("2021-04-10 00:00:00");

            // ACT
            var ranges = from.EachYearTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(1);
            ranges.First().Start.ShouldBe(from);
            ranges.First().End.ShouldBe(to);
        }

        [Fact]
        public void EachYearTo_CorrectlyReturnsFullYear()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-01-01 00:00:00");
            var to = DateTime.Parse("2022-01-01 00:00:00");

            // ACT
            var ranges = from.EachYearTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(1);
            ranges.First().Start.ShouldBe(from);
            ranges.First().End.ShouldBe(to);
        }

        [Fact]
        public void EachYearTo_CorrectlyReturnsFullYearAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-01-01 00:00:00");
            var to = DateTime.Parse("2022-03-03 00:00:00");

            // ACT
            var ranges = from.EachYearTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(2);
            ranges[0].Start.ShouldBe(from);
            ranges[0].End.ShouldBe(DateTime.Parse("2022-01-01 00:00:00"));

            ranges[0].End.ShouldBe(DateTime.Parse("2022-01-01 00:00:00"));
            ranges[1].End.ShouldBe(to);
        }

        [Fact]
        public void EachYearTo_CorrectlyReturnsMultipleFullYearsAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-01-05 00:50:00");
            var to = DateTime.Parse("2023-03-03 01:00:00");

            // ACT
            var ranges = from.EachYearTo(to).ToList();

            // ASSERT
            ranges.Count.ShouldBe(3);
            ranges[0].Start.ShouldBe(from);
            ranges[0].End.ShouldBe(DateTime.Parse("2022-01-01 00:00:00"));

            ranges[1].Start.ShouldBe(DateTime.Parse("2022-01-01 00:00:00"));
            ranges[1].End.ShouldBe(DateTime.Parse("2023-01-01 00:00:00"));
            
            ranges[2].Start.ShouldBe(DateTime.Parse("2023-01-01 00:00:00"));
            ranges[2].End.ShouldBe(to);
        }


    }
}
