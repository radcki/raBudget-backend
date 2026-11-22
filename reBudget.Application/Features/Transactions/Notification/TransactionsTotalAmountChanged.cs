using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Application.Infrastructure;
using raBudget.Application.Features.BudgetCategories.Notification;
using raBudget.Application.Features.Budgets.Notification;
using raBudget.Application.Features.Transactions.Command;
using raBudget.Common.Extensions;
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;

namespace raBudget.Application.Features.Transactions.Notification
{
    public class TransactionsTotalAmountChanged
    {
        public class Listener : IEventHandler<CreateTransaction.Notification>,
                                IEventHandler<RemoveTransaction.Notification>,
                                IEventHandler<AddSubTransaction.Notification>,
                                IEventHandler<RemoveSubTransaction.Notification>,
                                IEventHandler<UpdateSubTransactionAmount.Notification>,
                                IEventHandler<UpdateTransactionAmount.Notification>
        {
            public async Task HandleAsync(CreateTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await new TransactionsTotalAmountChanged.Notification
                                        {
                                            UserId = notification.UserId,
                                            ReferenceTransaction = notification.Transaction
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(RemoveTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await new TransactionsTotalAmountChanged.Notification
                                        {
                                            UserId = notification.UserId,
                                            ReferenceTransaction = notification.Transaction
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(AddSubTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await new TransactionsTotalAmountChanged.Notification
                                        {
                                            UserId = notification.UserId,
                                            ReferenceTransaction = notification.Transaction
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(RemoveSubTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await new TransactionsTotalAmountChanged.Notification
                                        {
                                            UserId = notification.UserId,
                                            ReferenceTransaction = notification.Transaction
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(UpdateSubTransactionAmount.Notification notification, CancellationToken cancellationToken)
            {
                await new TransactionsTotalAmountChanged.Notification
                                        {
                                            UserId = notification.UserId,
                                            ReferenceTransaction = notification.Transaction
                                        }.PublishAsync(cancellation: cancellationToken);
            }

            public async Task HandleAsync(UpdateTransactionAmount.Notification notification, CancellationToken cancellationToken)
            {
                await new TransactionsTotalAmountChanged.Notification
                                        {
                                            UserId = notification.UserId,
                                            ReferenceTransaction = notification.Transaction
                                        }.PublishAsync(cancellation: cancellationToken);
            }
        }

        public class Notification : UserNotification
        {
            public Transaction ReferenceTransaction { get; set; }
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
                var readDb = scope.ServiceProvider.GetRequiredService<IReadDbContext>();
                var balanceService = scope.ServiceProvider.GetRequiredService<BalanceService>();

                var category = readDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceTransaction.BudgetCategoryId);
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
                    await balanceService.CalculateBudgetCategoryBalance(category.BudgetCategoryId,
                                                                           dateTime.Year,
                                                                           dateTime.Month,
                                                                           cancellationToken);
                }

                await new BudgetCategoryBalanceChanged.Notification()
                                      {
                                          UserId = notification.UserId,
                                          BudgetCategoryId = notification.ReferenceTransaction.BudgetCategoryId
                                      }.PublishAsync(cancellation: cancellationToken);
                
                await balanceService.CalculateBudgetBalance(category.BudgetId, cancellationToken);

                await new BudgetBalanceChanged()
                                      {
                                          UserId = notification.UserId,
                                          BudgetId = category.BudgetId
                                      }.PublishAsync(cancellation: cancellationToken);
            }
        }
    }
}