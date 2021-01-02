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
    public class UpdateSubTransactionDateTime
    {
        public class Command : IRequest<Result>
        {
            public SubTransactionId SubTransactionId { get; set; }
            public DateTime TransactionDateTime { get; set; }
        }

        public class Result : SingleResponse<DateTime>
        {
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
                var subTransaction = await _writeDbContext.Transactions
                                                       .Include(x => x.SubTransactions)
                                                       .SelectMany(x=>x.SubTransactions)
                                                       .FirstOrDefaultAsync(x => x.SubTransactionId == request.SubTransactionId, cancellationToken: cancellationToken)
                                  ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

                if (!await _accessControlService.HasTransactionAccess(subTransaction.ParentTransactionId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }

                var oldDate = subTransaction.TransactionDate;
                subTransaction.SetTransactionDateTime(request.TransactionDateTime);

                await _writeDbContext.SaveChangesAsync(cancellationToken);


                _ = _mediator.Publish(new TransactionDateChanged.Notification()
                                      {
                                          OldDate = oldDate,
                                          NewDate = subTransaction.TransactionDate,
                                          ReferenceTransactionId = subTransaction.ParentTransactionId
                                      }, cancellationToken);

                return new Result() { Data = subTransaction.TransactionDate};
            }
        }
    }
}