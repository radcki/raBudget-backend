using System;
using System.Linq;
using FluentAssertions;
using raBudget.Common.Extensions;
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
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
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
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
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
            ranges.Should().HaveCount(2);
            ranges[0].Start.Should().Be(from);
            ranges[0].End.Should().Be(DateTime.Parse("2021-04-09 00:00:00"));

            ranges[0].End.Should().Be(DateTime.Parse("2021-04-09 00:00:00"));
            ranges[1].End.Should().Be(to);
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
            ranges.Should().HaveCount(3);
            ranges[0].Start.Should().Be(from);
            ranges[0].End.Should().Be(DateTime.Parse("2021-04-09 00:00:00"));

            ranges[1].Start.Should().Be(DateTime.Parse("2021-04-09 00:00:00"));
            ranges[1].End.Should().Be(DateTime.Parse("2021-04-10 00:00:00"));
            
            ranges[2].Start.Should().Be(DateTime.Parse("2021-04-10 00:00:00"));
            ranges[2].End.Should().Be(to);
        }


    }
}
