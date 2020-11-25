﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Entities;
using raBudget.Domain.Enums;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.Transactions.Command
{
    public class UpdateTransactionCategory
    {
        public class Command : IRequest<Result>
        {
            public TransactionId TransactionId { get; set; }
            public BudgetCategoryId BudgetCategoryId { get; set; }
        }

        public class Result : IdResponse<BudgetCategoryId>
        {
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

                return new Result()
                       {
                           Id = transaction.BudgetCategoryId
                       };
            }
        }
    }
}