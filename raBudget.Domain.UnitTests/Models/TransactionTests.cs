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
    public class TransactionTests : UnitTestsBase
    {
        [Fact]
        public void Throws_WhenChangingAmountToDifferentCurrency()
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

            var transaction = new Transaction(new TransactionId(RandomInt()),
                                              RandomString(5),
                                              budgetCategory,
                                              new MoneyAmount(budget.Currency, RandomInt()),
                                              DateTime.Now, DateTime.Now);
            var newAmount = new MoneyAmount(new Currency(eCurrencyCode.AFN), RandomInt());
            // Act
            Action act = () => transaction.SetAmount(newAmount);

            // Assert
            act.Should().Throw<BusinessException>();
        }

        [Fact]
        public void Should_SetTransactionDateWithoutTimePart()
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
            var transaction = new Transaction(new TransactionId(RandomInt()),
                                              RandomString(5),
                                              budgetCategory,
                                              new MoneyAmount(budget.Currency, RandomInt()),
                                              DateTime.Now, DateTime.Now);


            // Assert
            transaction.TransactionDateTime.Should().Be(DateTime.Today);
        }

        [Fact]
        public void Throws_WhenNewBudgetCategoryHasDifferentType()
        {
            // Arrange
            var budget = new Budget(new BudgetId(RandomInt()),
                                    RandomString(4),
                                    DateTime.Now,
                                    new Currency(eCurrencyCode.PLN));

            var icon = new BudgetCategoryIcon(new BudgetCategoryIconId(RandomInt()), RandomString(4));
            var budgetCategory1 = new BudgetCategory(new BudgetCategoryId(RandomInt()),
                                                    budget,
                                                    RandomString(4),
                                                    icon,
                                                    new List<BudgetCategory.BudgetedAmount>(),
                                                    eBudgetCategoryType.Income);
            var budgetCategory2 = new BudgetCategory(new BudgetCategoryId(RandomInt()),
                                                    budget,
                                                    RandomString(4),
                                                    icon,
                                                    new List<BudgetCategory.BudgetedAmount>(),
                                                    eBudgetCategoryType.Saving);

            var transaction = new Transaction(new TransactionId(RandomInt()),
                                              RandomString(5),
                                              budgetCategory1,
                                              new MoneyAmount(budget.Currency, RandomInt()),
                                              DateTime.Now, DateTime.Now);
            // Act
            Action act = () => transaction.SetBudgetCategory(budgetCategory2);

            // Assert
            act.Should().Throw<BusinessException>();
        }
    }
}