using MediatR;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Budgets.Notification
{
    public class BudgetBalanceChanged : INotification
    {
        public BudgetId BudgetId { get; set; }
    }
}
