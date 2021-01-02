using System;
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

namespace raBudget.Application.Features.BudgetCategories.Command
{
    public class UpdateBudgetCategoryIcon
    {
        public class Command : IRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public BudgetCategoryIconId BudgetCategoryIconId { get; set; }
        }

        public class Result : SingleResponse<IconDto>
        {
        }

        public class IconDto
        {
            public Guid IconId { get; set; }
            public string IconKey { get; set; }
        }

		public class Notification : INotification
		{
			public BudgetCategory ReferenceBudgetCategory { get; set; }
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
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(request.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetCategory = await _writeDbContext.BudgetCategories
                                                          .FirstOrDefaultAsync(x => x.BudgetCategoryId == request.BudgetCategoryId, cancellationToken);
                var icon = await _writeDbContext.BudgetCategoryIcons
                                                .FirstOrDefaultAsync(x => x.BudgetCategoryIconId == request.BudgetCategoryIconId, cancellationToken);
                budgetCategory.SetIcon(icon);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

				_ = _mediator.Publish(new Notification()
				{
					ReferenceBudgetCategory = budgetCategory,
				}, cancellationToken);

                return new Result()
                       {
                           Data = new IconDto()
                                  {
                                      IconId = icon.BudgetCategoryIconId.Value,
                                      IconKey = icon.IconKey
                                  }
                       };
            }
        }
    }
}