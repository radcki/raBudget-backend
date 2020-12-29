using System;
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
    public class UpdateTransactionAmount
    {
        public class Command : IRequest<Result>
        {
            public TransactionId TransactionId { get; set; }
            public MoneyAmount Amount { get; set; }
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
                
                if (!await _accessControlService.HasTransactionAccess(request.TransactionId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }
                var transaction = await _writeDbContext.Transactions
                                                       .FirstOrDefaultAsync(x => x.TransactionId == request.TransactionId, cancellationToken: cancellationToken)
                                  ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

                transaction.SetAmount(request.Amount);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                       {
                           Data = transaction.Amount
                       };
            }
        }
    }
}