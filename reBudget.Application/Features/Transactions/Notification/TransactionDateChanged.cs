using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Application.Features.BudgetCategories.Notification;
using raBudget.Application.Features.Transactions.Command;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Transactions.Notification
{
	public class TransactionDateChanged
	{
		public class Listener : INotificationHandler<UpdateTransactionDate.Notification>,
								INotificationHandler<UpdateSubTransactionDate.Notification>
		{
			private readonly IMediator _mediator;

			public Listener(IMediator mediator)
			{
				_mediator = mediator;
			}

			public async Task Handle(UpdateTransactionDate.Notification notification, CancellationToken cancellationToken)
			{
				await _mediator.Publish(new Notification()
				{
					ReferenceTransactionId = notification.Transaction.TransactionId,
					NewDate = notification.NewTransactionDate,
					OldDate = notification.OldTransactionDate
				}, cancellationToken);
			}

			public async Task Handle(UpdateSubTransactionDate.Notification notification, CancellationToken cancellationToken)
			{
				await _mediator.Publish(new Notification()
				{
					ReferenceTransactionId = notification.Transaction.TransactionId,
					NewDate = notification.NewSubTransactionDate,
					OldDate = notification.OldSubTransactionDate
				}, cancellationToken);
			}
		}

		public class Notification : INotification
		{
			public TransactionId ReferenceTransactionId { get; set; }
			public DateTime OldDate { get; set; }
			public DateTime NewDate { get; set; }
		}

		public class Handler : INotificationHandler<Notification>
		{
			private readonly BalanceService _balanceService;
			private readonly IReadDbContext _readDb;
			private readonly IMediator _mediator;

			public Handler(IServiceScopeFactory serviceScopeFactory, IMediator mediator)
			{
				_mediator = mediator;
				var serviceScope = serviceScopeFactory.CreateScope();
				_balanceService = serviceScope.ServiceProvider.GetService<BalanceService>();
				_readDb = serviceScope.ServiceProvider.GetService<IReadDbContext>();
			}

			/// <inheritdoc />
			public async Task Handle(Notification notification, CancellationToken cancellationToken)
			{
				if (notification.OldDate.Year == notification.NewDate.Year && notification.OldDate.Month == notification.NewDate.Month)
				{
					return;
				}

				var transaction = _readDb.Transactions.FirstOrDefault(x => x.TransactionId == notification.ReferenceTransactionId);
				if (transaction == null)
				{
					return;
				}

				await _balanceService.CalculateBudgetCategoryBalance(transaction.BudgetCategoryId,
																	   notification.OldDate.Year,
																	   notification.OldDate.Month,
																	   cancellationToken);

				await _balanceService.CalculateBudgetCategoryBalance(transaction.BudgetCategoryId,
																	   notification.NewDate.Year,
																	   notification.NewDate.Month,
																	   cancellationToken);

				_ = _mediator.Publish(new BudgetCategoryBalanceChanged.Notification()
				{
					BudgetCategoryId = transaction.BudgetCategoryId
				}, cancellationToken);
			}
		}
	}
}