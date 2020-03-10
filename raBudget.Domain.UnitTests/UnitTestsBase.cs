using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using raBudget.Domain.Enums;
using raBudget.Domain.Models;
using Xunit.Sdk;

namespace raBudget.Domain.UnitTests
{
    public class UnitTestsBase
    {
        protected string RandomString(int length)
        {
            const string chars = "abcdefghjklmnoprstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[new Random().Next(s.Length)])
                                        .ToArray());
        }

        protected int RandomInt(int min = 0, int max = 100)
        {
            return new Random().Next(min, max);
        }

        public Budget RandomBudget()
        {
            return new Budget(new Budget.Id(RandomInt()),
                              RandomString(4),
                              DateTime.Now,
                              new Currency(eCurrencyCode.PLN));
        }

        public BudgetCategory RandomBudgetCategory(Budget budget)
        {
            return RandomBudgetCategory(budget, eBudgetCategoryType.Spending);
        }
        public BudgetCategory RandomBudgetCategory(Budget budget, eBudgetCategoryType budgetCategoryType)
        {
            var icon = new BudgetCategoryIcon(new BudgetCategoryIcon.Id(RandomInt()), RandomString(4));
            return new BudgetCategory(new BudgetCategory.Id(RandomInt()),
                                  budget,
                                  RandomString(4),
                                  icon,
                                  new List<BudgetCategory.BudgetedAmount>(),
                                  budgetCategoryType);
        }
    }
}