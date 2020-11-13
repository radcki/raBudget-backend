using System;
using System.Collections.Generic;
using raBudget.Common.Resources;
using raBudget.Domain.BaseTypes;
using raBudget.Domain.Common;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Models;
using RLib.Localization;

namespace raBudget.Domain.Entities
{
    public class Budget
    {
        #region Constructors

        private Budget()
        {
            BudgetCategories = new List<BudgetCategory>();
        }

        public static Budget Create(string name, DateTime startingDate, Currency currency)
        {
            var budget = new Budget()
                         {
                             BudgetId = new Id()
                         };
            budget.SetName(name);
            budget.SetStartingDate(startingDate);
            budget.SetCurrency(currency);
            return budget;
        }

        #endregion

        #region Properties

        public Budget.Id BudgetId { get; private set; }
        public string Name { get; private set; }
        public string OwnerUserId { get; private set; }
        public DateTime StartingDate { get; private set; }
        public Currency Currency { get; private set; }

        public List<BudgetCategory> BudgetCategories { get; private set; }

        #endregion

        public class Id : IdValueBase<Guid>
        {
            public Id() : base(Guid.NewGuid()) { }
            public Id(Guid value) : base(value) { }
        }

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
            Currency = currency ?? throw new BusinessException("Budget currency is required");
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