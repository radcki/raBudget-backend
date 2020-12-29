using System;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.Entities
{
    public class BudgetCategoryBalance
    {
        private BudgetCategoryBalance(){}
        public static BudgetCategoryBalance Create(BudgetCategoryId budgetCategoryId, int year, int month)
        {
            return new BudgetCategoryBalance()
			{
				BudgetCategoryId = budgetCategoryId,
                Year = year,
                Month = month
			};
        }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public BudgetCategoryId BudgetCategoryId { get; private set; }
        public MoneyAmount BudgetedAmount { get; private set; }
        public MoneyAmount TransactionsTotal { get; private set; }
        public MoneyAmount AllocationsTotal { get; private set; }
        public DateTime UpdateTime { get; internal set; }

        public void Update(MoneyAmount budgetedAmount, MoneyAmount transactionsTotal, MoneyAmount allocationsTotal)
        {
            UpdateTime = DateTime.Now;

			BudgetedAmount = budgetedAmount;
			TransactionsTotal = transactionsTotal;
			AllocationsTotal = allocationsTotal;
		}

    }
}