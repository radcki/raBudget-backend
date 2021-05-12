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
    public class BudgetCategoryBalanceChanged
    {
        public class Listener : INotificationHandler<CreateAllocation.Notification>,
                                INotificationHandler<RemoveAllocation.Notification>,
                                INotificationHandler<UpdateAllocationAmount.Notification>,
                                INotificationHandler<UpdateAllocationDate.Notification>,
                                INotificationHandler<UpdateAllocationSourceCategory.Notification>,
                                INotificationHandler<UpdateAllocationTargetCategory.Notification>


        {
            private readonly IMediator _mediator;
            private readonly IWriteDbContext _writeDbContext;
            private readonly BalanceService _balanceService;

            public Listener(IServiceScopeFactory serviceScopeFactory)
            {
                var scope = serviceScopeFactory.CreateScope();
                _writeDbContext = scope.ServiceProvider.GetService<IWriteDbContext>();
                _mediator = scope.ServiceProvider.GetService<IMediator>();
                _balanceService = scope.ServiceProvider.GetService<BalanceService>();
            }


            /// <inheritdoc />
            public async Task Handle(CreateAllocation.Notification notification, CancellationToken cancellationToken)
            {
                var categories = new List<BudgetCategory>();
                categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.TargetBudgetCategoryId));
                if (notification.ReferenceAllocation.SourceBudgetCategoryId != null)
                {
                    categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.SourceBudgetCategoryId));
                }

                await RecalculateBalance(categories, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);

                foreach (var budgetCategory in categories)
                {
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }, cancellationToken);
                }
            }

            /// <inheritdoc />
            public async Task Handle(RemoveAllocation.Notification notification, CancellationToken cancellationToken)
            {
                var categories = new List<BudgetCategory>();
                categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.TargetBudgetCategoryId));
                if (notification.ReferenceAllocation.SourceBudgetCategoryId != null)
                {
                    categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.SourceBudgetCategoryId));
                }

                await RecalculateBalance(categories, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);

                foreach (var budgetCategory in categories)
                {
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }, cancellationToken);
                }
            }

            /// <inheritdoc />
            public async Task Handle(UpdateAllocationAmount.Notification notification, CancellationToken cancellationToken)
            {
                var categories = new List<BudgetCategory>();
                categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.TargetBudgetCategoryId));
                if (notification.ReferenceAllocation.SourceBudgetCategoryId != null)
                {
                    categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.SourceBudgetCategoryId));
                }

                await RecalculateBalance(categories, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);
                foreach (var budgetCategory in categories)
                {
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }, cancellationToken);
                }
            }

            public async Task Handle(UpdateAllocationDate.Notification notification, CancellationToken cancellationToken)
            {
                var categories = new List<BudgetCategory>();
                categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.TargetBudgetCategoryId));
                if (notification.ReferenceAllocation.SourceBudgetCategoryId != null)
                {
                    categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.ReferenceAllocation.SourceBudgetCategoryId));
                }


                await RecalculateBalance(categories, notification.NewAllocationDate.Year, notification.NewAllocationDate.Month, cancellationToken);

                if (notification.NewAllocationDate.Year != notification.OldAllocationDate.Year || notification.NewAllocationDate.Month != notification.OldAllocationDate.Month)
                {
                    await RecalculateBalance(categories, notification.OldAllocationDate.Year, notification.OldAllocationDate.Month, cancellationToken);
                }

                foreach (var budgetCategory in categories)
                {
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }, cancellationToken);
                }
            }


            /// <inheritdoc />
            public async Task Handle(UpdateAllocationSourceCategory.Notification notification, CancellationToken cancellationToken)
            {
                var categories = new List<BudgetCategory>();
                if (notification.OldBudgetCategoryId != null)
                {
                    categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.OldBudgetCategoryId));
                }

                if (notification.NewBudgetCategoryId != null)
                {
                    categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.NewBudgetCategoryId));
                }

                await RecalculateBalance(categories, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);
                foreach (var budgetCategory in categories)
                {
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }, cancellationToken);
                }
            }


            /// <inheritdoc />
            public async Task Handle(UpdateAllocationTargetCategory.Notification notification, CancellationToken cancellationToken)
            {
                var categories = new List<BudgetCategory>();
                if (notification.OldBudgetCategoryId != null)
                {
                    categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.OldBudgetCategoryId));
                }

                if (notification.NewBudgetCategoryId != null)
                {
                    categories.Add(_writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.NewBudgetCategoryId));
                }

                await RecalculateBalance(categories, notification.ReferenceAllocation.AllocationDate.Year, notification.ReferenceAllocation.AllocationDate.Month, cancellationToken);
                foreach (var budgetCategory in categories)
                {
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategoryId = budgetCategory.BudgetCategoryId
                                            }, cancellationToken);
                }
            }

            public async Task RecalculateBalance(IEnumerable<BudgetCategory> budgetCategories, int year, int month, CancellationToken cancellationToken)
            {
                foreach (var budgetCategory in budgetCategories)
                {
                    await _balanceService.CalculateBudgetCategoryBalance(budgetCategory.BudgetCategoryId, year, month, cancellationToken);
                }

                foreach (var budgetId in budgetCategories.Select(x => x.BudgetId).Distinct())
                {
                    await _balanceService.CalculateBudgetBalance(budgetId, cancellationToken);
                    await _mediator.Publish(new BudgetBalanceChanged()
                                            {
                                                BudgetId = budgetId
                                            }, cancellationToken);
                }
            }
        }

        public class Notification : INotification
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
        }
    }
}