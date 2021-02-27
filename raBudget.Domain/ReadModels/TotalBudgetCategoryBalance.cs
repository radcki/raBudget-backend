using raBudget.Domain.ValueObjects;

namespace raBudget.Domain.ReadModels
{
    public class TotalBudgetCategoryBalance
    {
        public BudgetCategoryId BudgetCategoryId { get; set; }
        public MoneyAmount TotalCategoryBalance { get; set; }
        public MoneyAmount ThisMonthTransactionsTotal { get; set; }
        public MoneyAmount BudgetLeftToEndOfYear { get; set; }

        public MoneyAmount TotalBudgetedAmount { get; set; }
        public MoneyAmount ThisMonthBudgetedAmount { get; set; }
        public MoneyAmount ThisYearBudgetedAmount { get; set; }
    }
}