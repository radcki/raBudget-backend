using System;
using FluentAssertions;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;
using Xunit;

namespace raBudget.Domain.UnitTests.Models
{
    public class BudgetTests : UnitTestsBase
    {
        [Fact]
        public void Throws_WhenCreatedWithoutName()
        {
            // Arrange

            // Act
            Action act1 = () => { Budget.Create(null, DateTime.Now, new Currency(eCurrencyCode.PLN)); };
            Action act2 = () => { Budget.Create("", DateTime.Now, new Currency(eCurrencyCode.PLN)); };
            Action act3 = () => { Budget.Create("   ", DateTime.Now, new Currency(eCurrencyCode.PLN)); };
            
            // Assert
            act1.Should().Throw<BusinessException>();
            act2.Should().Throw<BusinessException>();
            act3.Should().Throw<BusinessException>();
        }

        [Fact]
        public void Throws_WhenCreatedWithTooLongName()
        {
            // Arrange

            // Act
            Action act = () => { Budget.Create(RandomString(61), DateTime.Now, new Currency(eCurrencyCode.PLN)); };
            
            // Assert
            act.Should().Throw<BusinessException>();
        }

        [Fact]
        public void Throws_WhenCreatedWithoutDate()
        {
            // Arrange

            // Act
            Action act = () => { Budget.Create(RandomString(10), default, new Currency(eCurrencyCode.PLN)); };
            
            // Assert
            act.Should().Throw<BusinessException>();
        }

        [Fact]
        public void StartingDate_Should_BeSetToFirstDayOfMonth()
        {
            var budget = Budget.Create(RandomString(4), new DateTime(2020,4,4), new Currency(eCurrencyCode.PLN));
            budget.StartingDate.Should().Be(new DateTime(2020, 4, 1));
        }
    }
}
