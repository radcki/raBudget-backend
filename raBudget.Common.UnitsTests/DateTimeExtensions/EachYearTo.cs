using System;
using System.Linq;
using FluentAssertions;
using raBudget.Common.Extensions;
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
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
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
            ranges.Should().HaveCount(1);
            ranges.First().Start.Should().Be(from);
            ranges.First().End.Should().Be(to);
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
            ranges.Should().HaveCount(2);
            ranges[0].Start.Should().Be(from);
            ranges[0].End.Should().Be(DateTime.Parse("2022-01-01 00:00:00"));

            ranges[0].End.Should().Be(DateTime.Parse("2022-01-01 00:00:00"));
            ranges[1].End.Should().Be(to);
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
            ranges.Should().HaveCount(3);
            ranges[0].Start.Should().Be(from);
            ranges[0].End.Should().Be(DateTime.Parse("2022-01-01 00:00:00"));

            ranges[1].Start.Should().Be(DateTime.Parse("2022-01-01 00:00:00"));
            ranges[1].End.Should().Be(DateTime.Parse("2023-01-01 00:00:00"));
            
            ranges[2].Start.Should().Be(DateTime.Parse("2023-01-01 00:00:00"));
            ranges[2].End.Should().Be(to);
        }


    }
}
