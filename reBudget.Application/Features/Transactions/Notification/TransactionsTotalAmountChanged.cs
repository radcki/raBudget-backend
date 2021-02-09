using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Application.Features.BudgetCategories.Notification;
using raBudget.Application.Features.Budgets.Notification;
using raBudget.Application.Features.Transactions.Command;
using raBudget.Common.Extensions;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Transactions.Notification
{
    public class TransactionsTotalAmountChanged
    {
        public class Listener : INotificationHandler<CreateTransaction.Notification>,
                                INotificationHandler<RemoveTransaction.Notification>,
                                INotificationHandler<AddSubTransaction.Notification>,
                                INotificationHandler<RemoveSubTransaction.Notification>,
                                INotificationHandler<UpdateSubTransactionAmount.Notification>,
                                INotificationHandler<UpdateTransactionAmount.Notification>
        {
            private readonly IMediator _mediator;

            public Listener(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task Handle(CreateTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new TransactionsTotalAmountChanged.Notification
                                        {
                                            ReferenceTransaction = notification.Transaction
                                        }, cancellationToken);
            }

            public async Task Handle(RemoveTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new TransactionsTotalAmountChanged.Notification
                                        {
                                            ReferenceTransaction = notification.Transaction
                                        }, cancellationToken);
            }

            public async Task Handle(AddSubTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new TransactionsTotalAmountChanged.Notification
                                        {
                                            ReferenceTransaction = notification.Transaction
                                        }, cancellationToken);
            }

            public async Task Handle(RemoveSubTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new TransactionsTotalAmountChanged.Notification
                                        {
                                            ReferenceTransaction = notification.Transaction
                                        }, cancellationToken);
            }

            public async Task Handle(UpdateSubTransactionAmount.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new TransactionsTotalAmountChanged.Notification
                                        {
                                            ReferenceTransaction = notification.Transaction
                                        }, cancellationToken);
            }

            public async Task Handle(UpdateTransactionAmount.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new TransactionsTotalAmountChanged.Notification
                                        {
                                            ReferenceTransaction = notification.Transaction
                                        }, cancellationToken);
            }
        }

        public class Notification : INotification
        {
            public Transaction ReferenceTransaction { get; set; }
        }

        public class Handler : INotificationHandler<Notification>
        {
            private readonly BalanceService _balanceService;
            private readonly IReadDbContext _readDb;
            private readonly IMediator _mediator;

            public Handler(IServiceScopeFactory serviceScopeFactory)
            {
                var serviceScope = serviceScopeFactory.CreateScope();
                _mediator = serviceScope.ServiceProvider.GetService<IMediator>();
                _balanceService = serviceScope.ServiceProvider.GetService<BalanceService>();
                _readDb = serviceScope.ServiceProvider.GetService<IReadDbContext>();
            }


            /// <inheritdoc />
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                var category = _readDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceTransaction.BudgetCategoryId);
                if (category == null)
                {
                    return;
                }


                var dates = new List<DateTime>()
                            {
                                notification.ReferenceTransaction.TransactionDate
                            };
                if (notification.ReferenceTransaction.SubTransactions != null && notification.ReferenceTransaction.SubTransactions.Any())
                {
                    dates.AddRange(notification.ReferenceTransaction.SubTransactions.Select(x => x.TransactionDate));
                }

                foreach (var dateTime in dates.Select(x => x.StartOfMonth()).Distinct())
                {
                    await _balanceService.CalculateBudgetedCategoryBalance(category.BudgetCategoryId,
                                                                           dateTime.Year,
                                                                           dateTime.Month,
                                                                           cancellationToken);
                }

                await _mediator.Publish(new BudgetCategoryBalanceChanged()
                                      {
                                          BudgetCategoryId = notification.ReferenceTransaction.BudgetCategoryId
                                      }, cancellationToken);
                
                await _balanceService.CalculateBudgetBalance(category.BudgetId, cancellationToken);

                await _mediator.Publish(new BudgetBalanceChanged()
                                      {
                                          BudgetId = category.BudgetId
                                      }, cancellationToken);
            }
        }
    }
}