using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using raBudget.Application.Features.BudgetCategories.Command;
using raBudget.Application.Features.BudgetCategories.Notification;
using raBudget.Application.Features.Budgets.Notification;
using raBudget.Application.Features.Transactions.Command;
using raBudget.Application.Features.Transactions.Notification;
using raBudget.Domain.Interfaces;

namespace raBudget.Api.Hubs
{
    public static class TransactionNotificationEvents
    {
        public const string TransactionListChanged = "TRANSACTION_LIST_CHANGED";
        public const string TransactionUpdated = "TRANSACTION_UPDATED";
    }


    public class TransactionNotificationsHub : Hub
    {
        private readonly IUserContext _userContext;
        private readonly IHubContext<TransactionNotificationsHub> _hubContext;

        public TransactionNotificationsHub(IUserContext userContext, IHubContext<TransactionNotificationsHub> hubContext)
        {
            _userContext = userContext;
            _hubContext = hubContext;
        }

        public async Task Send(string eventKey, INotification payload)
        {
            await _hubContext.Clients.User(_userContext.UserId).SendAsync(eventKey, payload);
        }


        public class Listener : INotificationHandler<CreateTransaction.Notification>,
                                INotificationHandler<RemoveTransaction.Notification>,
                                INotificationHandler<AddSubTransaction.Notification>,
                                INotificationHandler<UpdateSubTransactionAmount.Notification>,
                                INotificationHandler<UpdateSubTransactionDate.Notification>,
                                INotificationHandler<UpdateSubTransactionDescription.Notification>,
                                INotificationHandler<RemoveSubTransaction.Notification>,
                                INotificationHandler<UpdateTransactionDate.Notification>,
                                INotificationHandler<UpdateTransactionCategory.Notification>,
                                INotificationHandler<UpdateTransactionDescription.Notification>,
                                INotificationHandler<RemoveBudgetCategory.Notification>
                                {
            private readonly TransactionNotificationsHub _transactionNotificationsHub;

            public Listener(TransactionNotificationsHub transactionNotificationsHub)
            {
                _transactionNotificationsHub = transactionNotificationsHub;
            }

            public async Task Handle(CreateTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionListChanged, notification);
            }

            /// <inheritdoc />
            public async Task Handle(RemoveTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionListChanged, notification);
            }
            /// <inheritdoc />
            public async Task Handle(RemoveBudgetCategory.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionListChanged, notification);
            }

            public async Task Handle(UpdateTransactionDescription.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionUpdated, notification);
            }

            public async Task Handle(AddSubTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionUpdated, notification);
            }

            public async Task Handle(UpdateSubTransactionAmount.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionUpdated, notification);
            }

            public async Task Handle(UpdateSubTransactionDate.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionUpdated, notification);
            }

            public async Task Handle(UpdateSubTransactionDescription.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionUpdated, notification);
            }

            public async Task Handle(UpdateTransactionDate.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionUpdated, notification);
            }

            public async Task Handle(UpdateTransactionCategory.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionUpdated, notification);
            }

            public async Task Handle(RemoveSubTransaction.Notification notification, CancellationToken cancellationToken)
            {
                await _transactionNotificationsHub.Send(TransactionNotificationEvents.TransactionUpdated, notification);
            }

        }
    }
}