using System;
using System.Collections.Generic;
using System.Linq;
using raBudget.Domain.BaseTypes;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.Models
{
    public class BudgetCategory
    {
        #region Constructors

        private BudgetCategory()
        {
        }

        public static BudgetCategory Create
        (Budget budget,
         string name,
         BudgetCategoryIcon icon,
         IEnumerable<BudgetedAmount> budgetedAmounts,
         eBudgetCategoryType budgetCategoryType)
        {
            var category = new BudgetCategory()
                           {
                               Icon = icon,
                               BudgetCategoryType = budgetCategoryType,
                               BudgetedAmounts = new List<BudgetedAmount>(),
                               BudgetId = budget.BudgetId
                           };

            category.SetName(name);
            category.SetIcon(icon);
            foreach (var budgetedAmount in budgetedAmounts)
            {
                category.AddBudgetedAmount(budgetedAmount.Amount, budgetedAmount.ValidFrom);
            }
            return category;
        }

        #endregion

        #region Properties

        public Id BudgetCategoryId { get; private set; }
        public Budget.Id BudgetId { get; private set; }
        public string Name { get; private set; }
        public BudgetCategoryIcon Icon { get; private set; }
        public IList<BudgetedAmount> BudgetedAmounts { get; private set; }
        public eBudgetCategoryType BudgetCategoryType { get; private set; }

        #endregion

        #region Methods

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(name.Trim()))
            {
                throw new BusinessException("Budget category name cannot be empty");
            }

            if (name.Length > 60)
            {
                throw new BusinessException("Budget category name must be shorter than 60 characters");
            }

            Name = name;
        }

        public void SetIcon(BudgetCategoryIcon icon)
        {
            Icon = icon;
        }

        public void AddBudgetedAmount(MoneyAmount amount, DateTime validFrom)
        {
            var budgetedAmount = new BudgetedAmount(this, amount, validFrom);
            if (BudgetedAmounts.Any(x => x.ValidFrom == budgetedAmount.ValidFrom))
            {
                throw new BusinessException("Another budgeted amount starting at same date");
            }

            BudgetedAmounts.Add(budgetedAmount);
            BudgetedAmounts = new List<BudgetedAmount>(BudgetedAmounts.OrderBy(x => x.ValidFrom));
        }

        public void RemoveBudgetedAmount(BudgetedAmount budgetedAmount)
        {
            var amountToRemove = BudgetedAmounts.FirstOrDefault(x => x == budgetedAmount);
            if (amountToRemove == null)
            {
                throw new BusinessException("Budgeted amount not found");
            }

            BudgetedAmounts.Remove(amountToRemove);
        }

        public void UpdateBudgetedAmount(BudgetedAmount budgetedAmount)
        {
            var amountToUpdate = BudgetedAmounts.FirstOrDefault(x => x == budgetedAmount);
            if (amountToUpdate == null)
            {
                throw new NotFoundException("Budgeted amount does not exist");
            }
            
            if (BudgetedAmounts.Any(x => x.BudgetedAmountId != budgetedAmount.BudgetedAmountId && x.ValidFrom == budgetedAmount.ValidFrom))
            {
                throw new BusinessException("Another budgeted amount starting at same date");
            }

            amountToUpdate.SetValidDate(budgetedAmount.ValidFrom);
            amountToUpdate.SetAmount(budgetedAmount.Amount);
        }

        #endregion

        public class Id : IdValueBase<int>
        {
            public Id(int value) : base(value)
            {
            }
        }

        #region Nested type: BudgetedAmount

        public class BudgetedAmount
        {
            #region Constructors

            public BudgetedAmount(BudgetCategory budgetCategory, MoneyAmount amount, DateTime validFrom)
            {
                Amount = amount;
                ValidFrom = validFrom;
                BudgetCategoryId = budgetCategory.BudgetCategoryId;
            }

            #endregion

            #region Properties

            public Id BudgetedAmountId { get; private set; }
            public BudgetCategory.Id BudgetCategoryId { get; private set; }
            public DateTime ValidFrom { get; private set; }
            public MoneyAmount Amount { get; private set; }

            #endregion

            #region Methods

            public void SetValidDate(DateTime newDate)
            {
                if (newDate == default)
                {
                    throw new BusinessException("Starting date must be set");
                }

                ValidFrom = newDate;
            }

            public void SetAmount(MoneyAmount newAmount)
            {
                if (newAmount.Amount < 0)
                {
                    throw new BusinessException("Value must be larger than 0");
                }

                Amount = newAmount;
            }

            #endregion

            public class Id : IdValueBase<int>
            {
                public Id(int value) : base(value)
                {
                }
            }
            }

        #endregion
    }
}