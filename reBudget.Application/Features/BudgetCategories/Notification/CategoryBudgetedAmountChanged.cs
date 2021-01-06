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

namespace raBudget.Application.Features.BudgetCategories.Notification
{
    public class CategoryBudgetedAmountChanged
    {
        public class Listener : INotificationHandler<CreateBudgetCategory.Notification>,
                                INotificationHandler<AddBudgetedAmount.Notification>,
                                INotificationHandler<RemoveBudgetCategory.Notification>,
                                INotificationHandler<RemoveBudgetedAmount.Notification>,
                                INotificationHandler<UpdateBudgetedAmountAmount.Notification>,
                                INotificationHandler<UpdateBudgetedAmountValidFrom.Notification>,
                                INotificationHandler<CreateAlloction.Notification>,
                                INotificationHandler<RemoveAllocation.Notification>,
                                INotificationHandler<UpdateAllocationAmount.Notification>,
                                INotificationHandler<UpdateAllocationSourceCategory.Notification>,
                                INotificationHandler<UpdateAllocationTargetCategory.Notification>


        {
            private readonly IMediator _mediator;
            private readonly IWriteDbContext _writeDbContext;

            public Listener(IMediator mediator, IServiceScopeFactory serviceScopeFactory)
            {
                _mediator = mediator;
                var scope = serviceScopeFactory.CreateScope();
                _writeDbContext = scope.ServiceProvider.GetService<IWriteDbContext>();
            }

            public async Task Handle(CreateBudgetCategory.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategory = notification.ReferenceBudgetCategory
                                        }, cancellationToken);
            }

            public async Task Handle(AddBudgetedAmount.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategory = notification.ReferenceBudgetCategory
                                        }, cancellationToken);
            }

            public async Task Handle(RemoveBudgetCategory.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategory = notification.ReferenceBudgetCategory
                                        }, cancellationToken);
            }

            public async Task Handle(RemoveBudgetedAmount.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategory = notification.ReferenceBudgetCategory
                                        }, cancellationToken);
            }

            public async Task Handle(UpdateBudgetedAmountAmount.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategory = notification.ReferenceBudgetCategory
                                        }, cancellationToken);
            }

            public async Task Handle(UpdateBudgetedAmountValidFrom.Notification notification, CancellationToken cancellationToken)
            {
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategory = notification.ReferenceBudgetCategory
                                        }, cancellationToken);
            }

            /// <inheritdoc />
            public async Task Handle(CreateAlloction.Notification notification, CancellationToken cancellationToken)
            {
                var targetCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.Allocation.TargetBudgetCategoryId);
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategory = targetCategory
                                        }, cancellationToken);
                if (notification.Allocation.SourceBudgetCategoryId != null)
                {
                    var sourceCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.Allocation.SourceBudgetCategoryId);
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategory = sourceCategory
                                            }, cancellationToken);
                }
            }

            /// <inheritdoc />
            public async Task Handle(RemoveAllocation.Notification notification, CancellationToken cancellationToken)
            {
                var targetCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.Allocation.TargetBudgetCategoryId);
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategory = targetCategory
                                        }, cancellationToken);
                if (notification.Allocation.SourceBudgetCategoryId != null)
                {
                    var sourceCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.Allocation.SourceBudgetCategoryId);
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategory = sourceCategory
                                            }, cancellationToken);
                }
            }

            /// <inheritdoc />
            public async Task Handle(UpdateAllocationAmount.Notification notification, CancellationToken cancellationToken)
            {
                var targetCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.Allocation.TargetBudgetCategoryId);
                await _mediator.Publish(new Notification()
                                        {
                                            BudgetCategory = targetCategory
                                        }, cancellationToken);
                if (notification.Allocation.SourceBudgetCategoryId != null)
                {
                    var sourceCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.Allocation.SourceBudgetCategoryId);
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategory = sourceCategory
                                            }, cancellationToken);
                }
            }


            /// <inheritdoc />
            public async Task Handle(UpdateAllocationSourceCategory.Notification notification, CancellationToken cancellationToken)
            {
                if (notification.OldBudgetCategoryId != null)
                {
                    var sourceCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.OldBudgetCategoryId);
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategory = sourceCategory
                                            }, cancellationToken);
                }

                if (notification.NewBudgetCategoryId != null)
                {
                    var sourceCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.NewBudgetCategoryId);
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategory = sourceCategory
                                            }, cancellationToken);
                }
            }


            /// <inheritdoc />
            public async Task Handle(UpdateAllocationTargetCategory.Notification notification, CancellationToken cancellationToken)
            {

                if (notification.OldBudgetCategoryId != null)
                {
                    var targetCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.OldBudgetCategoryId);
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategory = targetCategory
                                            }, cancellationToken);
                }

                if (notification.NewBudgetCategoryId != null)
                {
                    var targetCategory = _writeDbContext.BudgetCategories.FirstOrDefault(x => x.BudgetCategoryId == notification.NewBudgetCategoryId);
                    await _mediator.Publish(new Notification()
                                            {
                                                BudgetCategory = targetCategory
                                            }, cancellationToken);
                }
            }
        }

        public class Notification : INotification
        {
            public BudgetCategory BudgetCategory { get; set; }
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
                await _balanceService.CalculateBudgetBalance(notification.BudgetCategory.BudgetId, cancellationToken);
                await _balanceService.CalculateBudgetedCategoryBalance(notification.BudgetCategory.BudgetCategoryId, cancellationToken);

                await _mediator.Publish(new BudgetBalanceChanged()
                                        {
                                            BudgetId = notification.BudgetCategory.BudgetId
                                        }, cancellationToken);
                await _mediator.Publish(new BudgetCategoryBalanceChanged()
                                        {
                                            BudgetCategoryId = notification.BudgetCategory.BudgetCategoryId
                                        }, cancellationToken);
            }
        }
    }
}