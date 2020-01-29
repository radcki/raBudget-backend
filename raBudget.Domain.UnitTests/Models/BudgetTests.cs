using System;
using raBudget.Domain.Entities;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;
using Xunit;

namespace raBudget.Domain.UnitTests
{
    public class BudgetTests : UnitTestsBase
    {
        [Fact]
        public void Throws_WhenCreatedWithoutName()
        {
            Assert.Throws<BusinessException>(() =>
                                             {
                                                 var budget = new Budget(new BudgetId(RandomInt()), null, DateTime.Now);
                                             });
            Assert.Throws<BusinessException>(() =>
                                             {
                                                 var budget = new Budget(new BudgetId(RandomInt()), "", DateTime.Now);
                                             });
            Assert.Throws<BusinessException>(() =>
                                             {
                                                 var budget = new Budget(new BudgetId(RandomInt()), "   ", DateTime.Now);
                                             });
        }

        [Fact]
        public void Throws_WhenCreatedWithTooLongName()
        {
            Assert.Throws<BusinessException>(() =>
                                             {
                                                 var budget = new Budget(new BudgetId(RandomInt()), RandomString(61), DateTime.Now);
                                             });
        }

        [Fact]
        public void Throws_WhenCreatedWithoutDate()
        {
            Assert.Throws<BusinessException>(() =>
                                             {
                                                 var budget = new Budget(new BudgetId(RandomInt()), RandomString(10), default);
                                             });

        }

        [Fact]
        public void StartingDate_Should_BeSetToFirstDayOfMonth()
        {
            var budget = new Budget(new BudgetId(RandomInt()), RandomString(4), new DateTime(2020,4,4));
            Assert.Equal(new DateTime(2020,4,1), budget.StartingDate);
        }
    }
}
