using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using raBudget.Application.Features.Allocations.Command;
using raBudget.Application.Features.BudgetCategories.Command;
using raBudget.Application.Features.Budgets.Notification;
using raBudget.Application.Features.Transactions.Notification;
using raBudget.Common.Extensions;
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;

namespace raBudget.Application.Features.BudgetCategories.Notification
{
    public class CategoryBudgetedAmountChanged
    {
        public class Listener : INotificationHandler<CreateBudgetCategory.Notification>,
                                INotificationHandler<AddBudgetedAmount.Notification>,
                                INotificationHandler<RemoveBudgetCategory.Notification>,
                                INotificationHandler<RemoveBudgetedAmount.Notification>,
                                INotificationHandler<UpdateBudgetedAmountAmount.Notification>,
                                INotificationHandler<UpdateBudgetedAmountValidFrom.Notification>


        {
            private readonly IMediator _mediator;
            private readonly IWriteDbContext _writeDbContext;

            public Listener(IServiceScopeFactory serviceScopeFactory)
            {
                var scope = serviceScopeFactory.CreateScope();
                _writeDbContext = scope.ServiceProvider.GetService<IWriteDbContext>();
                _mediator = scope.ServiceProvider.GetService<IMediator>();
            }

            public async Task Handle(CreateBudgetCategory.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }, cancellationToken);
            }

            public async Task Handle(AddBudgetedAmount.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }, cancellationToken);
            }

            public async Task Handle(RemoveBudgetCategory.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }, cancellationToken);
            }

            public async Task Handle(RemoveBudgetedAmount.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }, cancellationToken);
            }

            public async Task Handle(UpdateBudgetedAmountAmount.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }, cancellationToken);
            }

            public async Task Handle(UpdateBudgetedAmountValidFrom.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategories = new List<BudgetCategory>() {notification.ReferenceBudgetCategory}
                                        }, cancellationToken);
            }
            }

        public class Notification : INotification
        {
            public List<BudgetCategory> BudgetCategories { get; set; }
        }

        public class Handler : INotificationHandler<Notification>
        {
            private readonly BalanceService _balanceService;
            private readonly IMediator _mediator;

            public Handler(IServiceScopeFactory serviceScopeFactory)
            {
                var serviceScope = serviceScopeFactory.CreateScope();
                _balanceService = serviceScope.ServiceProvider.GetService<BalanceService>();
                _mediator = serviceScope.ServiceProvider.GetService<IMediator>();
            }


            /// <inheritdoc />
            public async Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                foreach (var budgetCategory in notification.BudgetCategories)
                {
                    await _balanceService.CalculateBudgetCategoryBalance(budgetCategory.BudgetCategoryId, cancellationToken);
                    await _mediator.Publish(new BudgetCategoryBalanceChanged.Notification()
                                            {
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }, cancellationToken);
                }


                foreach (var budgetId in notification.BudgetCategories.Select(x => x.BudgetId).Distinct())
                {
                    await _balanceService.CalculateBudgetBalance(budgetId, cancellationToken);
                    await _mediator.Publish(new BudgetBalanceChanged()
                                            {
                                                BudgetId = budgetId
                                            }, cancellationToken);
                }
            }
        }
    }
}