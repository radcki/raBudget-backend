﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Common.Response;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using raBudget.Domain.ValueObjects;
using RLib.Localization;

namespace raBudget.Application.Features.Allocations.Command
{
    public class RemoveAllocation
    {
        public class Command : IRequest<Result>
        {
            public AllocationId AllocationId { get; set; }
        }

        public class Result : BaseResponse
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
                var allocation = await _writeDbContext.Allocations
                                                       .FirstOrDefaultAsync(x => x.AllocationId == request.AllocationId, cancellationToken: cancellationToken)
                                  ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

                if (!await _accessControlService.HasAllocationAccess(allocation.AllocationId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.AllocationNotFound));
                }

                _writeDbContext.Allocations.Remove(allocation);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                return new Result() { };
            }
        }
    }
}