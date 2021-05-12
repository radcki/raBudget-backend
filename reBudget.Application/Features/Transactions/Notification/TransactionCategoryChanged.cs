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
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Transactions.Notification
{
    public class TransactionCategoryChanged
    {
        public class Listener : INotificationHandler<UpdateTransactionCategory.Notification>
		{
			private readonly IMediator _mediator;

			public Listener(IMediator mediator)
			{
				_mediator = mediator;
			}

			public async Task Handle(UpdateTransactionCategory.Notification notification, CancellationToken cancellationToken)
			{
				await _mediator.Publish(new Notification()
				{
					NewBudgetCategoryId = notification.NewBudgetCategoryId,
					OldBudgetCategoryId = notification.OldBudgetCategoryId,
					ReferenceTransaction = notification.Transaction
				}, cancellationToken);
			}
		}

        public class Notification : INotification
        {
            public Transaction ReferenceTransaction { get; set; }
            public BudgetCategoryId OldBudgetCategoryId { get; set; }
            public BudgetCategoryId NewBudgetCategoryId { get; set; }
        }

        public class Handler : INotificationHandler<Notification>
        {
            private readonly BalanceService _balanceService;
			private readonly IMediator _mediator;

            public Handler(IServiceScopeFactory serviceScopeFactory, IMediator mediator)
            {
				_mediator = mediator;
				var serviceScope = serviceScopeFactory.CreateScope();
                _balanceService = serviceScope.ServiceProvider.GetService<BalanceService>();
            }


            /// <inheritdoc />
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                await _balanceService.CalculateBudgetCategoryBalance(notification.OldBudgetCategoryId,
                                                                       notification.ReferenceTransaction.TransactionDate.Year,
                                                                       notification.ReferenceTransaction.TransactionDate.Month,
                                                                       cancellationToken);

                await _balanceService.CalculateBudgetCategoryBalance(notification.NewBudgetCategoryId,
																	   notification.ReferenceTransaction.TransactionDate.Year,
																	   notification.ReferenceTransaction.TransactionDate.Month,
																	   cancellationToken);


				_ = _mediator.Publish(new BudgetCategoryBalanceChanged.Notification()
				{
					BudgetCategoryId = notification.OldBudgetCategoryId
				}, cancellationToken);

				_ = _mediator.Publish(new BudgetCategoryBalanceChanged.Notification()
				{
					BudgetCategoryId = notification.NewBudgetCategoryId
				}, cancellationToken);
			}
		}
    }
}
