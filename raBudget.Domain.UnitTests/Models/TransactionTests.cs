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
            var budget = Budget.Create(RandomString(4),
                                       DateTime.Now,
                                       new Currency(eCurrencyCode.PLN));

            var icon = new BudgetCategoryIcon(new BudgetCategoryIconId(Guid.NewGuid()), RandomString(4));
            var budgetCategory = BudgetCategory.Create(budget,
                                                       RandomString(4),
                                                       icon,
                                                       eBudgetCategoryType.Income);

            var transaction = Transaction.Create(RandomString(5),
                                                 budgetCategory,
                                                 new MoneyAmount(budget.Currency.CurrencyCode, RandomInt()),
                                                 DateTime.Now);
            var newAmount = new MoneyAmount(eCurrencyCode.AFN, RandomInt());
            // Act
            Action act = () => transaction.SetAmount(newAmount);

            // Assert
            act.Should().Throw<BusinessException>();
        }

        [Fact]
        public void Should_SetTransactionDateWithoutTimePart()
        {
            // Arrange
            var budget = Budget.Create(RandomString(4),
                                       DateTime.Now,
                                       new Currency(eCurrencyCode.PLN));

            var icon = new BudgetCategoryIcon(new BudgetCategoryIconId(Guid.NewGuid()), RandomString(4));
            var budgetCategory = BudgetCategory.Create(budget,
                                                       RandomString(4),
                                                       icon,
                                                       eBudgetCategoryType.Income);


            // Act
            var transaction = Transaction.Create(RandomString(5),
                                                 budgetCategory,
                                                 new MoneyAmount(budget.Currency.CurrencyCode, RandomInt()),
                                                 DateTime.Now);


            // Assert
            transaction.TransactionDate.Should().Be(DateTime.Today);
        }

    }
}