using System;
using raBudget.Common.Resources;
using raBudget.Domain.Common;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Domain.Entities
{
    public class Budget
    {
        #region Constructors

        private Budget()
        {
            BudgetCategories = new BudgetCategoryCollection();
        }

        public static Budget Create(string name, DateTime startingDate, Currency currency)
        {
            var budget = new Budget()
                         {
                             BudgetId = new BudgetId()
                         };
            budget.SetName(name);
            budget.SetStartingDate(startingDate);
            budget.SetCurrency(currency);
            return budget;
        }

        #endregion

        #region Properties

        public BudgetId BudgetId { get; private set; }
        public string Name { get; private set; }
        public string OwnerUserId { get; private set; }
        public DateTime StartingDate { get; private set; }
        public eCurrencyCode CurrencyCode { get; set; }
        public Currency Currency => new Currency(CurrencyCode);

        public BudgetCategoryCollection BudgetCategories { get; private set; }

        #endregion
        
        #region Methods

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(name.Trim()))
            {
                throw new BusinessException("Budged name cannot be empty");
            }

            if (name.Length > 60)
            {
                throw new BusinessException("Budged name must be shorter than 60 characters");
            }

            Name = name;
        }

        public void SetStartingDate(DateTime startingDate)
        {
            if (startingDate == default)
            {
                throw new BusinessException("Budget starting date is required");
            }

            StartingDate = DateHelpers.FirstDayOfMonth(startingDate);
        }

        private void SetCurrency(Currency currency)
        {
            CurrencyCode = currency?.CurrencyCode ?? throw new BusinessException("Budget currency is required");
        }

        public void SetOwner(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new BusinessException(Localization.For(() => ErrorMessages.UserRequired));
            }

            OwnerUserId = userId;
        }

        #endregion
    }
}