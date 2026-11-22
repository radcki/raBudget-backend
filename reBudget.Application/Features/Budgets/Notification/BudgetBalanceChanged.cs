using raBudget.Application.Infrastructure;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Budgets.Notification
{
    public class BudgetBalanceChanged : UserNotification
    {
        public BudgetId BudgetId { get; set; }
    }
}
