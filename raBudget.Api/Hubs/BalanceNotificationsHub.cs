using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using raBudget.Application.Features.BudgetCategories.Notification;
using raBudget.Application.Features.Budgets.Notification;
using raBudget.Application.Features.Transactions.Notification;
using raBudget.Domain.Interfaces;

namespace raBudget.Api.Hubs
{
	public static class BalanceNotificationEvents
	{
		public const string TotalBalanceChanged = "TOTAL_BALANCE_CHANGED";
		public const string BudgetCategoryBalanceChanged = "BUDGET_CATEGORY_BALANCE_CHANGED";
	}


	public class BalanceNotificationsHub : Hub
	{
		private readonly IUserContext _userContext;
		private readonly IHubContext<BalanceNotificationsHub> _hubContext;

		public BalanceNotificationsHub(IUserContext userContext, IHubContext<BalanceNotificationsHub> hubContext)
		{
			_userContext = userContext;
			_hubContext = hubContext;
		}

		public async Task Send(string eventKey, INotification payload)
		{
			await _hubContext.Clients.User(_userContext.UserId).SendAsync(eventKey, payload);
		}


		public class Listener : INotificationHandler<BudgetCategoryBalanceChanged.Notification>, 
								INotificationHandler<BudgetBalanceChanged>
		{
			private readonly BalanceNotificationsHub _balanceNotificationsHub;

			public Listener(BalanceNotificationsHub balanceNotificationsHub)
			{
				_balanceNotificationsHub = balanceNotificationsHub;
			}

			public async Task Handle(BudgetCategoryBalanceChanged.Notification notification, CancellationToken cancellationToken)
			{
				await _balanceNotificationsHub.Send(BalanceNotificationEvents.BudgetCategoryBalanceChanged, notification);
			}

			public async Task Handle(BudgetBalanceChanged notification, CancellationToken cancellationToken)
			{
				await _balanceNotificationsHub.Send(BalanceNotificationEvents.TotalBalanceChanged, notification);
			}
		}
	}

}
