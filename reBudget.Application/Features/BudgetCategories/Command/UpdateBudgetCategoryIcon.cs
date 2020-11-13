using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.BudgetCategories.Command
{
    public class UpdateBudgetCategoryIcon
    {
        public class Command : IRequest<Result>
        {
            public Guid BudgetCategoryId { get; set; }
            public Guid IconId { get; set; }
        }

        public class Result
        {
            public Guid IconId { get; set; }
            public string IconKey { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly AccessControlService _accessControlService;

            public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService)
            {
                _writeDbContext = writeDbContext;
                _accessControlService = accessControlService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(new BudgetCategory.Id(request.BudgetCategoryId)))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetCategory = await _writeDbContext.BudgetCategories
                                                          .FirstOrDefaultAsync(x => x.BudgetCategoryId.Value == request.BudgetCategoryId, cancellationToken);
                var icon = await _writeDbContext.BudgetCategoryIcons
                                                .FirstOrDefaultAsync(x => x.IconId.Value == request.IconId, cancellationToken);
                budgetCategory.SetIcon(icon);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                       {
                           IconId = icon.IconId.Value,
                           IconKey = icon.IconKey
                       };
            }
        }
    }
}