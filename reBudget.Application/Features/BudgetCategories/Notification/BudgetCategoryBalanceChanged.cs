using MediatR;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.BudgetCategories.Notification
{
    public class BudgetCategoryBalanceChanged : INotification
    {
        public BudgetCategoryId BudgetCategoryId { get; set; }
    }
}
