using System;
using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.Entities
{
    public class BudgetBalance
    {
        private BudgetBalance(){}
        public static BudgetBalance Create(BudgetId budgetId)
        {
            return new BudgetBalance(){BudgetId = budgetId};
        }
        public BudgetId BudgetId { get; private set; }
        public MoneyAmount TotalBalance { get; private set; }
        public MoneyAmount UnassignedFunds { get; private set; }
        public MoneyAmount SpendingTotal { get; private set; }
        public MoneyAmount IncomeTotal { get; private set; }
        public MoneyAmount SavingTotal { get; private set; }
        public DateTime UpdateTime { get; internal set; }

        public void Update(MoneyAmount totalBalance, MoneyAmount unassignedFunds, MoneyAmount spendingTotal, MoneyAmount incomeTotal, MoneyAmount savingTotal)
        {
            UpdateTime = DateTime.Now;

            TotalBalance = totalBalance;
            UnassignedFunds = unassignedFunds;
            SpendingTotal = spendingTotal;
            IncomeTotal = incomeTotal;
            SavingTotal = savingTotal;
        }

    }
}