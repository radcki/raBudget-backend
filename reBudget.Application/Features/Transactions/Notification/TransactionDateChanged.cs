using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Domain.Enums;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.Transactions.Notification
{
    public class TransactionDateChanged
    {
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
                if (notification.OldDate.Year == notification.NewDate.Year && notification.OldDate.Month == notification.NewDate.Month)
                {
                    return;
                }
                var transaction = _readDb.Transactions.FirstOrDefault(x => x.TransactionId == notification.ReferenceTransactionId);
                if (transaction == null)
                {
                    return;
                }

                await _balanceService.CalculateBudgetedCategoryBalance(transaction.BudgetCategoryId,
                                                                       notification.OldDate.Year,
                                                                       notification.OldDate.Month,
                                                                       cancellationToken);

                await _balanceService.CalculateBudgetedCategoryBalance(transaction.BudgetCategoryId,
                                                                       notification.NewDate.Year,
                                                                       notification.NewDate.Month,
                                                                       cancellationToken);
            }

            #endregion
        }
    }
}
