using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using raBudget.Application.Infrastructure;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.Transactions.Command
{
    public class UpdateTransactionCategory
    {
        public class Command : IApplicationRequest<Result>
        {
            public TransactionId TransactionId { get; set; }
            public BudgetCategoryId BudgetCategoryId { get; set; }
        }

        public class Result : SingleResponse<BudgetCategoryId>
        {
        }
        
        public class Notification: UserNotification
		{
			public Transaction Transaction { get; set; }
            public BudgetCategoryId OldBudgetCategoryId { get; set; }
            public BudgetCategoryId NewBudgetCategoryId { get; set; }
		}

        public class Handler : ICommandHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;
			private readonly PostCommitEventPublisher _eventPublisher;

            public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService, PostCommitEventPublisher eventPublisher)
            {
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
				_eventPublisher = eventPublisher;
			}

            public async Task<Result> ExecuteAsync(Command request, CancellationToken cancellationToken)
            {
                if (!await _accessControlService.HasTransactionAccess(request.TransactionId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }

                if (!await _accessControlService.HasBudgetCategoryAccessAsync(request.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var transaction = await _writeDbContext.Transactions
                                                       .FirstAsync(x => x.TransactionId == request.TransactionId, cancellationToken: cancellationToken);

                var oldBudgetCategory = await _writeDbContext.BudgetCategories
                                                             .FirstAsync(x => x.BudgetCategoryId == transaction.BudgetCategoryId, cancellationToken: cancellationToken);

                var newBudgetCategory = await _writeDbContext.BudgetCategories
                                                          .FirstAsync(x => x.BudgetCategoryId == request.BudgetCategoryId, cancellationToken: cancellationToken);

                if (oldBudgetCategory.BudgetCategoryType != newBudgetCategory.BudgetCategoryType)
                {
                    throw new BusinessException(Localization.For(() => ErrorMessages.NotSameBudgetCategoryType));
                }

                transaction.SetBudgetCategory(newBudgetCategory);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

				_ = _eventPublisher.PublishAsync(new Notification()
				{
					NewBudgetCategoryId = newBudgetCategory.BudgetCategoryId,
					OldBudgetCategoryId = oldBudgetCategory.BudgetCategoryId,
					Transaction = transaction
				}, cancellationToken);

                return new Result()
                       {
                           Data = transaction.BudgetCategoryId
                       };
            }
        }
    }
}