using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.ReadModels
{
    public class BudgetBalance
    {
        public BudgetId BudgetId { get; set; }
        public MoneyAmount TotalBalance { get; set; }
        public MoneyAmount UnassignedFunds { get; set; }
        public MoneyAmount SpendingTotal { get; set; }
        public MoneyAmount IncomeTotal { get; set; }
        public MoneyAmount SavingTotal { get; set; }
    }
}