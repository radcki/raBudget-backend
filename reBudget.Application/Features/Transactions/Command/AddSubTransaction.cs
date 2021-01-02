﻿using System;
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
    public class AddSubTransaction
    {
        public class Command : IRequest<Result>
        {
            public TransactionId TransactionId { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; }
            public DateTime TransactionDate { get; set; }
        }

        public class Result : IdResponse<SubTransactionId>
        {
        }

		public class Notification : INotification
		{
			public Transaction Transaction { get; set; }
			public SubTransaction SubTransaction { get; set; }
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
                var transaction = await _writeDbContext.Transactions
                                                       .FirstOrDefaultAsync(x => x.TransactionId == request.TransactionId, cancellationToken: cancellationToken)
                                  ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

                if (!await _accessControlService.HasBudgetCategoryAccessAsync(transaction.BudgetCategoryId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }

                var subTransaction = transaction.AddSubTransaction(request.Description,
                                                                   new MoneyAmount(transaction.Amount.CurrencyCode, request.Amount),
                                                                   request.TransactionDate);

                await _writeDbContext.SaveChangesAsync(cancellationToken);


                _ = _mediator.Publish(new Notification()
                                      {
                                          Transaction = transaction,
                                          SubTransaction = subTransaction
                                      }, cancellationToken);

                return new Result()
                       {
                           Id = subTransaction.SubTransactionId
                };
            }
        }
    }
}