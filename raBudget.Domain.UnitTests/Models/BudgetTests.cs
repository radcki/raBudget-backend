using System;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using Shouldly;
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
            act1.ShouldThrow<BusinessException>();
            act2.ShouldThrow<BusinessException>();
            act3.ShouldThrow<BusinessException>();
        }

        [Fact]
        public void Throws_WhenCreatedWithTooLongName()
        {
            // Arrange

            // Act
            Action act = () => { Budget.Create(RandomString(61), DateTime.Now, new Currency(eCurrencyCode.PLN)); };
            
            // Assert
            act.ShouldThrow<BusinessException>();
        }

        [Fact]
        public void Throws_WhenCreatedWithoutDate()
        {
            // Arrange

            // Act
            Action act = () => { Budget.Create(RandomString(10), default, new Currency(eCurrencyCode.PLN)); };
            
            // Assert
            act.ShouldThrow<BusinessException>();
        }

        [Fact]
        public void StartingDate_Should_BeSetToFirstDayOfMonth()
        {
            var budget = Budget.Create(RandomString(4), new DateTime(2020,4,4), new Currency(eCurrencyCode.PLN));
            budget.StartingDate.ShouldBe(new DateTime(2020, 4, 1));
        }
    }
}
