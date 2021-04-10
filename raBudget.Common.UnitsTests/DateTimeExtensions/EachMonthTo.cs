using System;
using System.Linq;
using FluentAssertions;
using raBudget.Common.Extensions;
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
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
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
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
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
            ranges.Should().HaveCount(2);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(DateTime.Parse("2021-05-01 00:00:00"));
            
            ranges.Last().Start.Should().Be(DateTime.Parse("2021-05-01 00:00:00"));
            ranges.Last().End.Should().Be(to);
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
            ranges.Should().HaveCount(4);
            ranges[0].Start.Should().Be(from);
            ranges[0].End.Should().Be(DateTime.Parse("2021-05-01 00:00:00"));

            ranges[1].Start.Should().Be(DateTime.Parse("2021-05-01 00:00:00"));
            ranges[1].End.Should().Be(DateTime.Parse("2021-06-01 00:00:00"));

            ranges[2].Start.Should().Be(DateTime.Parse("2021-06-01 00:00:00"));
            ranges[2].End.Should().Be(DateTime.Parse("2021-07-01 00:00:00"));

            ranges[3].Start.Should().Be(DateTime.Parse("2021-07-01 00:00:00"));
            ranges[3].End.Should().Be(to);
        }


    }
}
