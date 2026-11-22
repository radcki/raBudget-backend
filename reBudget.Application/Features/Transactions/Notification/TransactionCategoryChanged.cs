using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Application.Infrastructure;
using raBudget.Application.Features.BudgetCategories.Notification;
using raBudget.Application.Features.Transactions.Command;
using raBudget.Domain.Entities;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Transactions.Notification
{
    public class TransactionCategoryChanged
    {
        public class Listener : IEventHandler<UpdateTransactionCategory.Notification>
		{
			public async Task HandleAsync(UpdateTransactionCategory.Notification notification, CancellationToken cancellationToken)
			{
				await new Notification()
				{
					UserId = notification.UserId,
					NewBudgetCategoryId = notification.NewBudgetCategoryId,
					OldBudgetCategoryId = notification.OldBudgetCategoryId,
					ReferenceTransaction = notification.Transaction
				}.PublishAsync(cancellation: cancellationToken);
			}
		}

        public class Notification : UserNotification
        {
            public Transaction ReferenceTransaction { get; set; }
            public BudgetCategoryId OldBudgetCategoryId { get; set; }
            public BudgetCategoryId NewBudgetCategoryId { get; set; }
        }

        public class Handler : IEventHandler<Notification>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }


            /// <inheritdoc />
            public async Task HandleAsync(Notification notification, CancellationToken cancellationToken)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

                await balanceService.CalculateBudgetCategoryBalance(notification.OldBudgetCategoryId,
                                                                       notification.ReferenceTransaction.TransactionDate.Year,
                                                                       notification.ReferenceTransaction.TransactionDate.Month,
                                                                       cancellationToken);

                await balanceService.CalculateBudgetCategoryBalance(notification.NewBudgetCategoryId,
																	   notification.ReferenceTransaction.TransactionDate.Year,
																	   notification.ReferenceTransaction.TransactionDate.Month,
																	   cancellationToken);


				await new BudgetCategoryBalanceChanged.Notification()
				{
					UserId = notification.UserId,
					BudgetCategoryId = notification.OldBudgetCategoryId
				}.PublishAsync(cancellation: cancellationToken);

				await new BudgetCategoryBalanceChanged.Notification()
				{
					UserId = notification.UserId,
					BudgetCategoryId = notification.NewBudgetCategoryId
				}.PublishAsync(cancellation: cancellationToken);
			}
		}
    }
}
