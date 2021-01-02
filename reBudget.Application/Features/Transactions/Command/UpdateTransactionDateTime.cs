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
    public class UpdateTransactionDateTime
    {
        public class Command : IRequest<Result>
        {
            public TransactionId TransactionId { get; set; }
            public DateTime TransactionDate { get; set; }
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
                
                if (!await _accessControlService.HasTransactionAccess(request.TransactionId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }
                var transaction = await _writeDbContext.Transactions
                                                       .FirstOrDefaultAsync(x => x.TransactionId == request.TransactionId, cancellationToken: cancellationToken)
                                  ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                var oldDate = transaction.TransactionDate;
                transaction.SetTransactionDate(request.TransactionDate);

                await _writeDbContext.SaveChangesAsync(cancellationToken);
                _ = _mediator.Publish(new TransactionDateChanged.Notification()
                                      {
                                          OldDate = oldDate,
                                          NewDate = transaction.TransactionDate,
                                          ReferenceTransactionId = transaction.TransactionId
                                      }, cancellationToken);

                return new Result()
                       {
                           Data = transaction.TransactionDate
                       };
            }
        }
    }
}