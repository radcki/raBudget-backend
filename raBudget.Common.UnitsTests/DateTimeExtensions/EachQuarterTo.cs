using System;
using System.Linq;
using FluentAssertions;
using raBudget.Common.Extensions;
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
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
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
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
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
            ranges.Should().HaveCount(2);
            ranges[0].Start.Should().Be(from);
            ranges[0].End.Should().Be(DateTime.Parse("2021-07-01 00:00:00"));

            ranges[0].End.Should().Be(DateTime.Parse("2021-07-01 00:00:00"));
            ranges[1].End.Should().Be(to);
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
            ranges.Should().HaveCount(3);
            ranges[0].Start.Should().Be(from);
            ranges[0].End.Should().Be(DateTime.Parse("2021-07-01 00:00:00"));

            ranges[1].Start.Should().Be(DateTime.Parse("2021-07-01 00:00:00"));
            ranges[1].End.Should().Be(DateTime.Parse("2021-10-01 00:00:00"));
            
            ranges[2].Start.Should().Be(DateTime.Parse("2021-10-01 00:00:00"));
            ranges[2].End.Should().Be(to);
        }


    }
}
