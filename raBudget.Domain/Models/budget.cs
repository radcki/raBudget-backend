using System;
using raBudget.Domain.Entities;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.Models
{
    public class Budget
    {
        #region Constructors

        private Budget(Currency currency)
        {
            Currency = currency;
        }

        public Budget(BudgetId budgetId, string name, DateTime startingDate, Currency currency)
        {
            BudgetId = budgetId;
            SetName(name);
            SetStartingDate(startingDate);
            SetCurrency(currency);
        }

        #endregion

        #region Properties

        public BudgetId BudgetId { get; }

        public string Name { get; private set; }
        public DateTime StartingDate { get; private set; }
        public Currency Currency { get; private set; }

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
            if (startingDate == null || startingDate == default)
            {
                throw new BusinessException("Budget starting date is required");
            }

            StartingDate = new DateTime(startingDate.Year, startingDate.Day, 1);
        }

        private void SetCurrency(Currency currency)
        {
            Currency = currency ?? throw new BusinessException("Budget currency is required");
        }

        #endregion
    }
}