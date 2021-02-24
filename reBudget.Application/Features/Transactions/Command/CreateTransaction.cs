using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Application.Features.Transactions.Notification;
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
    public class CreateTransaction
    {
        public class Command : IRequest<Result>
        {
            public BudgetCategoryId BudgetCategoryId { get; set; }
            public MoneyAmount Amount { get; set; }
            public string Description { get; set; }
            public DateTime TransactionDate { get; set; }
        }

        public class Result : IdResponse<TransactionId>
        {
        }

		public class Notification : INotification
		{
            public Transaction Transaction { get; set; }
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
                var budgetCategoryId = request.BudgetCategoryId;
                if (!await _accessControlService.HasBudgetCategoryAccessAsync(budgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetCategoryNotFound));
                }

                var budgetCategory = _writeDbContext.BudgetCategories
                                                    .First(x => x.BudgetCategoryId == request.BudgetCategoryId);

                var transaction = Transaction.Create(request.Description,
                                                     budgetCategory,
                                                     request.Amount,
                                                     request.TransactionDate);
                _writeDbContext.Transactions.Add(transaction);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                _ = _mediator.Publish(new Notification() {
                                          Transaction = transaction
                                      }, cancellationToken);

                return new Result()
                       {
                           Id = transaction.TransactionId
                       };
            }
        }
    }
}