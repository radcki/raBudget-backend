using System;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.ReadModels
{
	public class BudgetCategoryBalance
	{
		public int Year { get; set; }
		public int Month { get; set; }
		public BudgetCategoryId BudgetCategoryId { get; set; }
		public MoneyAmount BudgetedAmount { get; set; }
		public MoneyAmount TransactionsTotal { get; set; }
		public MoneyAmount AllocationsTotal { get; set; }
	}
}