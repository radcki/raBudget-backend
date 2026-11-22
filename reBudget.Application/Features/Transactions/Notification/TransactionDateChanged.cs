using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Application.Infrastructure;
using raBudget.Application.Features.BudgetCategories.Notification;
using raBudget.Application.Features.Transactions.Command;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Transactions.Notification
{
	public class TransactionDateChanged
	{
		public class Listener : IEventHandler<UpdateTransactionDate.Notification>,
								IEventHandler<UpdateSubTransactionDate.Notification>
		{
			public async Task HandleAsync(UpdateTransactionDate.Notification notification, CancellationToken cancellationToken)
			{
				await new Notification()
				{
					UserId = notification.UserId,
					ReferenceTransactionId = notification.Transaction.TransactionId,
					NewDate = notification.NewTransactionDate,
					OldDate = notification.OldTransactionDate
				}.PublishAsync(cancellation: cancellationToken);
			}

			public async Task HandleAsync(UpdateSubTransactionDate.Notification notification, CancellationToken cancellationToken)
			{
				await new Notification()
				{
					UserId = notification.UserId,
					ReferenceTransactionId = notification.Transaction.TransactionId,
					NewDate = notification.NewSubTransactionDate,
					OldDate = notification.OldSubTransactionDate
				}.PublishAsync(cancellation: cancellationToken);
			}
		}

		public class Notification : UserNotification
		{
			public TransactionId ReferenceTransactionId { get; set; }
			public DateTime OldDate { get; set; }
			public DateTime NewDate { get; set; }
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
				if (notification.OldDate.Year == notification.NewDate.Year && notification.OldDate.Month == notification.NewDate.Month)
				{
					return;
				}

				await using var scope = _scopeFactory.CreateAsyncScope();
				var readDb = scope.ServiceProvider.GetRequiredService<IReadDbContext>();
				var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

				var transaction = readDb.Transactions.FirstOrDefault(x => x.TransactionId == notification.ReferenceTransactionId);
				if (transaction == null)
				{
					return;
				}

				await balanceService.CalculateBudgetCategoryBalance(transaction.BudgetCategoryId,
																	   notification.OldDate.Year,
																	   notification.OldDate.Month,
																	   cancellationToken);

				await balanceService.CalculateBudgetCategoryBalance(transaction.BudgetCategoryId,
																	   notification.NewDate.Year,
																	   notification.NewDate.Month,
																	   cancellationToken);

				await new BudgetCategoryBalanceChanged.Notification()
				{
					UserId = notification.UserId,
					BudgetCategoryId = transaction.BudgetCategoryId
				}.PublishAsync(cancellation: cancellationToken);
			}
		}
	}
}