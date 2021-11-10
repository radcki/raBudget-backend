using System;
using System.Collections.Generic;
using raBudget.Common.Entities;
using raBudget.Common.Resources;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Domain.Entities
{
	public class Allocation : BaseEntity
	{
		private Allocation()
		{
		}

		public static Allocation Create
		(string description,
		 BudgetCategory targetBudgetCategory,
		 BudgetCategory sourceBudgetCategory,
		 MoneyAmount amount,
		 DateTime allocationDate)
		{
			var transaction = new Allocation
			{
				AllocationId = new AllocationId(),
				Description = description
			};

			transaction.SetTargetBudgetCategory(targetBudgetCategory);
			transaction.SetSourceBudgetCategory(sourceBudgetCategory);
			transaction.SetAmount(amount);
			transaction.SetAllocationDate(allocationDate);
			transaction.CreationDateTime = DateTime.Now;

			return transaction;
		}

		public AllocationId AllocationId { get; private set; }
		public string Description { get; private set; }
		public BudgetCategoryId TargetBudgetCategoryId { get; private set; }
		public BudgetCategoryId SourceBudgetCategoryId { get; private set; }
		public MoneyAmount Amount { get; private set; }
		public DateTime AllocationDate { get; private set; }
		public DateTime CreationDateTime { get; private set; }

		public void SetAmount(MoneyAmount newAmount)
		{
			if (Amount != null && newAmount.Currency != Amount.Currency)
			{
				throw new BusinessException("New amount must be of same currency");
			}

			Amount = newAmount;
		}

		public void SetAllocationDate(DateTime newAllocationDate)
		{
			AllocationDate = newAllocationDate.Date;
		}

		public void SetDescription(string description)
		{
			description = description.Trim();
			if (string.IsNullOrEmpty(description))
			{
				throw new BusinessException(Localization.For(() => ErrorMessages.TransactionDescriptionEmpty));
			}

			Description = description;
		}

		public void SetTargetBudgetCategory(BudgetCategory budgetCategory)
		{
			if (budgetCategory == default)
			{
				throw new BusinessException(Localization.For(() => ErrorMessages.BudgetCategoryEmpty));
			}

			TargetBudgetCategoryId = budgetCategory.BudgetCategoryId;
		}

		public void SetSourceBudgetCategory(BudgetCategory budgetCategory)
		{
			SourceBudgetCategoryId = budgetCategory?.BudgetCategoryId;
		}
	}
}