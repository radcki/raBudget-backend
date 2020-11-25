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
    public class UpdateSubTransactionAmount
    {
        public class Command : IRequest<Result>
        {
            public SubTransactionId SubTransactionId { get; set; }
            public decimal Amount { get; set; }
        }

        public class Result : SingleResponse<MoneyAmount>
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
                var subTransaction = await _writeDbContext.Transactions
                                                          .Include(x => x.SubTransactions)
                                                          .SelectMany(x => x.SubTransactions)
                                                          .FirstOrDefaultAsync(x => x.SubTransactionId == request.SubTransactionId, cancellationToken: cancellationToken)
                                     ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

                if (!await _accessControlService.HasTransactionAccess(subTransaction.ParentTransactionId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }

                subTransaction.SetAmount(new MoneyAmount(subTransaction.Amount.Currency, request.Amount));

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result() {Data = subTransaction.Amount};
            }
        }
    }
}