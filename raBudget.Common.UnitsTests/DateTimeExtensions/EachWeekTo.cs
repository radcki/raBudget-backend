using System;
using System.Linq;
using FluentAssertions;
using raBudget.Common.Extensions;
using Xunit;

namespace raBudget.Common.UnitsTests.DateTimeExtensions
{
    public class EachWeekTo
    {
        [Fact]
        public void EachWeekTo_CorrectlyReturnsPartialWeek()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-08 00:00:00");
            var to = DateTime.Parse("2021-04-10 00:00:00");

            // ACT
            var ranges = from.EachWeekTo(to, DayOfWeek.Monday).ToList();

            // ASSERT
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
        }

        [Fact]
        public void EachWeekTo_CorrectlyReturnsFullWeek()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-05 00:00:00");
            var to = DateTime.Parse("2021-04-12 00:00:00");

            // ACT
            var ranges = from.EachWeekTo(to, DayOfWeek.Monday).ToList();

            // ASSERT
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
        }

        [Fact]
        public void EachWeekTo_CorrectlyReturnsFullWeekAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-05 00:00:00");
            var to = DateTime.Parse("2021-04-15 00:00:00");

            // ACT
            var ranges = from.EachWeekTo(to, DayOfWeek.Monday).ToList();

            // ASSERT
            ranges.Should().HaveCount(2);
            ranges[0].Start.Should().Be(from);
            ranges[0].End.Should().Be(DateTime.Parse("2021-04-12 00:00:00"));
            
            ranges[1].Start.Should().Be(DateTime.Parse("2021-04-12 00:00:00"));
            ranges[1].End.Should().Be(to);
        }

        [Fact]
        public void EachWeekTo_CorrectlyReturnsMultipleFullWeeksAndPeriod()
        {
            // ARRANGE
            var from = DateTime.Parse("2021-04-06 07:00:00");
            var to = DateTime.Parse("2021-04-28 00:40:00");

            // ACT
            var ranges = from.EachWeekTo(to, DayOfWeek.Monday).ToList();

            // ASSERT
            ranges.Should().HaveCount(4);
            ranges[0].Start.Should().Be(from);
            ranges[0].End.Should().Be(DateTime.Parse("2021-04-12 00:00:00"));

            ranges[1].Start.Should().Be(DateTime.Parse("2021-04-12 00:00:00"));
            ranges[1].End.Should().Be(DateTime.Parse("2021-04-19 00:00:00"));

            ranges[2].Start.Should().Be(DateTime.Parse("2021-04-19 00:00:00"));
            ranges[2].End.Should().Be(DateTime.Parse("2021-04-26 00:00:00"));

            ranges[3].Start.Should().Be(DateTime.Parse("2021-04-26 00:00:00"));
            ranges[3].End.Should().Be(to);
        }


    }
}
