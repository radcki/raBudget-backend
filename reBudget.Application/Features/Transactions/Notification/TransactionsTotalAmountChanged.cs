using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
        public class Notification : INotification
        {
            public Transaction ReferenceTransaction { get; set; }
        }

        public class Handler : INotificationHandler<Notification>
        {
            private readonly BalanceService _balanceService;
            private readonly IReadDbContext _readDb;

            public Handler(IServiceScopeFactory serviceScopeFactory)
            {
                var serviceScope = serviceScopeFactory.CreateScope();
                _balanceService = serviceScope.ServiceProvider.GetService<BalanceService>();
                _readDb = serviceScope.ServiceProvider.GetService<IReadDbContext>();
            }

            #region Implementation of INotificationHandler<in Notification>

            /// <inheritdoc />
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                var category = _readDb.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceTransaction.BudgetCategoryId);
                if (category == null)
                {
                    return;
                }

                await _balanceService.CalculateBudgetBalance(category.BudgetId, cancellationToken);

                var dates = new List<DateTime>()
                            {
                                notification.ReferenceTransaction.TransactionDate
                            };
                if (notification.ReferenceTransaction.SubTransactions != null && notification.ReferenceTransaction.SubTransactions.Any())
                {
                    dates.AddRange(notification.ReferenceTransaction.SubTransactions.Select(x=>x.TransactionDate));
                }
                foreach (var dateTime in dates.Select(x=>x.StartOfMonth()).Distinct())
                {
                    await _balanceService.CalculateBudgetedCategoryBalance(category.BudgetCategoryId,
                                                                           dateTime.Year,
                                                                           dateTime.Month,
                                                                           cancellationToken);
                }
                
            }

            #endregion
        }
    }
}
