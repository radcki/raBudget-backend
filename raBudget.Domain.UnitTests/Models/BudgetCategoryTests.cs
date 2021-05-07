using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;
using Xunit;

namespace raBudget.Domain.UnitTests.Models
{
    public class BudgetCategoryTests : UnitTestsBase
    {
        [Fact]
        public void AddsBudgetedAmount_WhenAddingBudgetedAmount()
        {
            // Arrange
            var budget = RandomBudget();
            var budgetCategory = RandomBudgetCategory(budget);

            // Act
            var amount = new MoneyAmount(budget.Currency.CurrencyCode, RandomInt());
            budgetCategory.AddBudgetedAmount(amount, DateTime.Today);

            // Assert
            budgetCategory.BudgetedAmounts.Should().HaveCount(1);
            budgetCategory.BudgetedAmounts.FirstOrDefault().Amount.Should().Be(amount);
            budgetCategory.BudgetedAmounts.FirstOrDefault().ValidFrom.Should().Be(DateTime.Today);
        }

        [Fact]
        public void Throws_WhenAddingBudgetedAmountWithSameDate()
        {
            // Arrange
            var budget = RandomBudget();
            var budgetCategory = RandomBudgetCategory(budget);

            // Act
            budgetCategory.AddBudgetedAmount(new MoneyAmount(budget.Currency.CurrencyCode, RandomInt()), DateTime.Today);
            Action act = () => budgetCategory.AddBudgetedAmount(new MoneyAmount(budget.Currency.CurrencyCode, RandomInt()), DateTime.Today);

            // Assert
            act.Should().Throw<BusinessException>();
        }

        [Fact]
        public void Throws_WhenRemovingNotExistingBudgetedAmount()
        {
            // Arrange
            var budget = RandomBudget();

            var budgetCategory = RandomBudgetCategory(budget);
            var budgetCategory2 = RandomBudgetCategory(budget);
            var budgetedAmount = budgetCategory2.AddBudgetedAmount(new MoneyAmount(budget.Currency.CurrencyCode, RandomInt()), DateTime.Today);
           
            // Act
            Action act = ()=> budgetCategory.RemoveBudgetedAmount(budgetedAmount);

            // Assert
            act.Should().Throw<BusinessException>();
        }

        [Fact]
        public void UpdatesAmount_WhenUpdatingBudgetedAmount()
        {
            // Arrange
            var budget = RandomBudget();

            var budgetCategory = RandomBudgetCategory(budget);
            var amount = new MoneyAmount(budget.Currency.CurrencyCode, RandomInt());
            budgetCategory.AddBudgetedAmount(amount, DateTime.Today);
            var amountToUpdate = budgetCategory.BudgetedAmounts.ToList().First();
            amountToUpdate.SetAmount(new MoneyAmount(budget.Currency.CurrencyCode, RandomInt()));;

            // Act
            Action act = () => budgetCategory.UpdateBudgetedAmount(amountToUpdate);

            // Assert
            budgetCategory.BudgetedAmounts.FirstOrDefault().Amount.Should().Be(amountToUpdate.Amount);
        }

        [Fact]
        public void UpdatesDate_WhenUpdatingBudgetedAmount()
        {
            // Arrange
            var budget = RandomBudget();

            var budgetCategory = RandomBudgetCategory(budget);
            var amount = new MoneyAmount(budget.Currency.CurrencyCode, RandomInt());
            budgetCategory.AddBudgetedAmount(amount, DateTime.Today);
            var amountToUpdate = budgetCategory.BudgetedAmounts.ToList().First();
            amountToUpdate.SetValidFromDate(DateTime.Today.AddMonths(-1)); ;

            // Act
            Action act = () => budgetCategory.UpdateBudgetedAmount(amountToUpdate);

            // Assert
            budgetCategory.BudgetedAmounts.FirstOrDefault().ValidFrom.Should().Be(amountToUpdate.ValidFrom);
        }

        [Fact]
        public void Throws_WhenUpdatingNotExistingBudgetedAmount()
        {
            // Arrange
            var budget = RandomBudget();

            var budgetCategory = RandomBudgetCategory(budget);
            var budgetCategory2 = RandomBudgetCategory(budget);
            var budgetedAmount = budgetCategory2.AddBudgetedAmount(new MoneyAmount(budget.Currency.CurrencyCode, RandomInt()), DateTime.Today);

            // Act
            Action act = () => budgetCategory.UpdateBudgetedAmount(budgetedAmount);

            // Assert
            act.Should().Throw<NotFoundException>();
        }
    }
}