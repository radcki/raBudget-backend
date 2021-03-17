using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.TransactionTemplates.Command
{
    public class UpdateTransactionTemplateCategory
    {
        public class Command : IRequest<Result>
        {
            public TransactionTemplateId TransactionTemplateId { get; set; }
            public BudgetCategoryId BudgetCategoryId { get; set; }
        }

        public class Result : SingleResponse<BudgetCategoryId>
        {
        }

        public class Notification : INotification
        {
            public TransactionTemplate TransactionTemplate { get; set; }
            public BudgetCategoryId OldBudgetCategoryId { get; set; }
            public BudgetCategoryId NewBudgetCategoryId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;
            private readonly IMediator _mediator;

            public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService, IMediator mediator)
            {
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
                _mediator = mediator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!await _accessControlService.HasTransactionTemplateAccess(request.TransactionTemplateId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }

                if (!await _accessControlService.HasBudgetCategoryAccessAsync(request.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var transactionTemplate = await _writeDbContext.TransactionTemplates
                                                               .FirstAsync(x => x.TransactionTemplateId == request.TransactionTemplateId, cancellationToken: cancellationToken);

                var oldBudgetCategory = await _writeDbContext.BudgetCategories
                                                             .FirstAsync(x => x.BudgetCategoryId == transactionTemplate.BudgetCategoryId, cancellationToken: cancellationToken);

                var newBudgetCategory = await _writeDbContext.BudgetCategories
                                                             .FirstAsync(x => x.BudgetCategoryId == request.BudgetCategoryId, cancellationToken: cancellationToken);

                if (oldBudgetCategory.BudgetCategoryType != newBudgetCategory.BudgetCategoryType)
                {
                    throw new BusinessException(Localization.For(() => ErrorMessages.NotSameBudgetCategoryType));
                }

                transactionTemplate.SetBudgetCategory(newBudgetCategory);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                _ = _mediator.Publish(new Notification()
                                      {
                                          NewBudgetCategoryId = newBudgetCategory.BudgetCategoryId,
                                          OldBudgetCategoryId = oldBudgetCategory.BudgetCategoryId,
                                          TransactionTemplate = transactionTemplate
                                      }, cancellationToken);

                return new Result()
                       {
                           Data = transactionTemplate.BudgetCategoryId
                       };
            }
        }
    }
}