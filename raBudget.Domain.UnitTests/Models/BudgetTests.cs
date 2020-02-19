using System;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;
using Xunit;

namespace raBudget.Domain.UnitTests.Models
{
    public class BudgetTests : UnitTestsBase
    {
        [Fact]
        public void Throws_WhenCreatedWithoutName()
        {
            Assert.Throws<BusinessException>(() => { new Budget(new BudgetId(RandomInt()), null, DateTime.Now, new Currency(eCurrencyCode.PLN)); });
            Assert.Throws<BusinessException>(() => { new Budget(new BudgetId(RandomInt()), "", DateTime.Now, new Currency(eCurrencyCode.PLN)); });
            Assert.Throws<BusinessException>(() => { new Budget(new BudgetId(RandomInt()), "   ", DateTime.Now, new Currency(eCurrencyCode.PLN)); });
        }

        [Fact]
        public void Throws_WhenCreatedWithTooLongName()
        {
            Assert.Throws<BusinessException>(() => { new Budget(new BudgetId(RandomInt()), RandomString(61), DateTime.Now, new Currency(eCurrencyCode.PLN)); });
        }

        [Fact]
        public void Throws_WhenCreatedWithoutDate()
        {
            Assert.Throws<BusinessException>(() => { new Budget(new BudgetId(RandomInt()), RandomString(10), default, new Currency(eCurrencyCode.PLN)); });

        }

        [Fact]
        public void StartingDate_Should_BeSetToFirstDayOfMonth()
        {
            var budget = new Budget(new BudgetId(RandomInt()), RandomString(4), new DateTime(2020,4,4), new Currency(eCurrencyCode.PLN));
            Assert.Equal(new DateTime(2020,4,1), budget.StartingDate);
        }
    }
}
