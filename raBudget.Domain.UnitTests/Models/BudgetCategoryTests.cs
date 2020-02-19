using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;
using Xunit;

namespace raBudget.Domain.UnitTests.Models
{
    public class BudgetCategoryTests : UnitTestsBase
    {
        [Fact]
        public void Throws_WhenAddingBudgetedAmountWithSameDate()
        {
            // Arrange
            var budget = new Budget(new BudgetId(RandomInt()),
                                    RandomString(4),
                                    DateTime.Now,
                                    new Currency(eCurrencyCode.PLN));
            var icon = new BudgetCategoryIcon(new BudgetCategoryIconId(RandomInt()), RandomString(4));
            var budgetCategory = new BudgetCategory(new BudgetCategoryId(RandomInt()),
                                                    budget,
                                                    RandomString(4),
                                                    icon,
                                                    new List<BudgetCategory.BudgetedAmount>(),
                                                    eBudgetCategoryType.Income);

            // Act
            budgetCategory.AddBudgetedAmount(new MoneyAmount(budget.Currency, RandomInt()), DateTime.Today);

            // Assert
            Assert.Throws<BusinessException>(() => budgetCategory.AddBudgetedAmount(new MoneyAmount(budget.Currency, RandomInt()), DateTime.Today));
        }

        [Fact]
        public void Throws_WhenRemovingNotExistingBudgetedAmount()
        {
            // Arrange
            var budget = new Budget(new BudgetId(RandomInt()),
                                    RandomString(4),
                                    DateTime.Now,
                                    new Currency(eCurrencyCode.PLN));

            var icon = new BudgetCategoryIcon(new BudgetCategoryIconId(RandomInt()), RandomString(4));
            var budgetCategory = new BudgetCategory(new BudgetCategoryId(RandomInt()),
                                                    budget,
                                                    RandomString(4),
                                                    icon,
                                                    new List<BudgetCategory.BudgetedAmount>(),
                                                    eBudgetCategoryType.Income);
            var budgetedAmount = new BudgetCategory.BudgetedAmount(budgetCategory, new MoneyAmount(budget.Currency, RandomInt()), DateTime.Today);
            // Act
            Action act = ()=> budgetCategory.RemoveBudgetedAmount(budgetedAmount);

            // Assert
            act.Should().Throw<BusinessException>();
        }
    }
}