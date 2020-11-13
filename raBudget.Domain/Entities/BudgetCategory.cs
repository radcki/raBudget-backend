using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using raBudget.Common.Resources;
using raBudget.Domain.BaseTypes;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Models;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.Entities
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
         eBudgetCategoryType budgetCategoryType)
        {
            var category = new BudgetCategory()
                           {
                               BudgetCategoryId = new Id(),
                               BudgetCategoryType = budgetCategoryType,
                               BudgetId = budget.BudgetId
                           };
            category.SetIcon(icon);
            category.SetName(name);
            category.SetIcon(icon);
            return category;
        }

        #endregion

        #region Properties

        public Id BudgetCategoryId { get; private set; }
        public Budget.Id BudgetId { get; private set; }
        public string Name { get; private set; }
        public BudgetCategoryIcon.Id IconId { get; private set; }
        public BudgetedAmountCollection BudgetedAmounts { get; private set; }
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
            IconId = icon?.IconId;
        }

        public BudgetedAmount AddBudgetedAmount(MoneyAmount amount, DateTime validFrom)
        {
            var budgetedAmount = BudgetedAmount.Create(this, amount, validFrom);
            if (BudgetedAmounts.Any(x => x.ValidFrom == budgetedAmount.ValidFrom))
            {
                throw new BusinessException("Another budgeted amount starting at same date");
            }

            BudgetedAmounts.Add(budgetedAmount);
            return budgetedAmount;
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

            amountToUpdate.SetValidFromDate(budgetedAmount.ValidFrom);
            amountToUpdate.SetAmount(budgetedAmount.Amount);
        }

        #endregion

        public class Id : IdValueBase<Guid>
        {
            public Id() : base(Guid.NewGuid()) { }
            public Id(Guid value) : base(value) { }
        }

        #region Nested type: BudgetedAmount

        public class BudgetedAmount
        {
            private DateTime _validFrom;

            #region Constructors

            private BudgetedAmount()
            {
            }

            internal static BudgetedAmount Create(BudgetCategory budgetCategory, MoneyAmount amount, DateTime validFrom)
            {
                var instance = new BudgetedAmount()
                               {
                                   BudgetCategoryId = budgetCategory.BudgetCategoryId
                               };
                instance.SetAmount(amount);
                instance.SetValidFromDate(validFrom);
                return instance;
            }

            #endregion

            #region Properties

            public Id BudgetedAmountId { get; private set; }
            public BudgetCategory.Id BudgetCategoryId { get; private set; }

            public DateTime ValidFrom
            {
                get => _validFrom;
                private set
                {
                    _validFrom = value;
                    ValidFromChanged?.Invoke(this, value);
                }
            }

            public DateTime? ValidTo { get; private set; }
            public MoneyAmount Amount { get; private set; }

            public event EventHandler<DateTime> ValidFromChanged;

            #endregion

            #region Methods

            public void SetValidFromDate(DateTime newDate)
            {
                if (newDate == default)
                {
                    throw new BusinessException("Starting date must be set");
                }

                ValidFrom = newDate;
            }

            internal void SetValidToDate(DateTime? newDate)
            {
                if (newDate != null && ValidTo < ValidFrom)
                {
                    throw new BusinessException(ErrorMessages.EndDateBeforeStartDate);
                }

                ValidTo = newDate;
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

            public class Id : IdValueBase<Guid>
            {
                public Id() : base(Guid.NewGuid()) { }
                public Id(Guid value) : base(value) { }
            }
        }

        public sealed class BudgetedAmountCollection : ObservableCollection<BudgetedAmount>
        {
            public BudgetedAmountCollection()
            {
                CollectionChanged += (sender, args) =>
                                     {
                                         if (args.NewItems == null)
                                         {
                                             return;
                                         }

                                         foreach (var newItems in args.NewItems)
                                         {
                                             if (newItems is BudgetedAmount budgetedAmount)
                                             {
                                                 budgetedAmount.ValidFromChanged += (o, time) => SetValidToDates();
                                             }
                                         }
                                     };
            }

            public new void Add(BudgetedAmount item)
            {
                base.Add(item);
                SetValidToDates();
            }

            private void SetValidToDates()
            {
                var sorted = this.OrderBy(x => x.ValidFrom).ToList();
                for (var i = 0; i < sorted.Count; i++)
                {
                    var current = sorted[i];
                    var next = sorted.ElementAtOrDefault(i + 1);
                    if (next != null)
                    {
                        current.SetValidToDate(next.ValidFrom.AddDays(-1));
                    }
                    else
                    {
                        current.SetValidToDate(null);
                    }
                }
            }
        }

        #endregion
    }
}