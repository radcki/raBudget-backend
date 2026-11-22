using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.SignalR;
using raBudget.Application.Infrastructure;
using raBudget.Application.Features.BudgetCategories.Notification;
using raBudget.Application.Features.Budgets.Notification;

namespace raBudget.Api.Hubs
{
	public static class BalanceNotificationEvents
	{
		public const string TotalBalanceChanged = "TOTAL_BALANCE_CHANGED";
		public const string BudgetCategoryBalanceChanged = "BUDGET_CATEGORY_BALANCE_CHANGED";
	}


	public class BalanceNotificationsHub : Hub
	{
		public class Listener : IClientNotificationHandler,
		                        IEventHandler<BudgetCategoryBalanceChanged.Notification>,
								IEventHandler<BudgetBalanceChanged>
		{
			private readonly IHubContext<BalanceNotificationsHub> _hubContext;

			public Listener(IHubContext<BalanceNotificationsHub> hubContext)
			{
				_hubContext = hubContext;
			}

			public async Task HandleAsync(BudgetCategoryBalanceChanged.Notification notification, CancellationToken cancellationToken)
			{
				await SendAsync(BalanceNotificationEvents.BudgetCategoryBalanceChanged, notification, cancellationToken);
			}

			public async Task HandleAsync(BudgetBalanceChanged notification, CancellationToken cancellationToken)
			{
				await SendAsync(BalanceNotificationEvents.TotalBalanceChanged, notification, cancellationToken);
			}

			private Task SendAsync(string eventKey, UserNotification notification, CancellationToken cancellationToken)
			{
			    return _hubContext.Clients.User(notification.UserId).SendAsync(eventKey, notification, cancellationToken);
			}
		}
	}

}
