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

namespace raBudget.Application.Features.Allocations.Command
{
    public class UpdateAllocationDateTime
    {
        public class Command : IRequest<Result>
        {
            public AllocationId AllocationId { get; set; }
            public DateTime AllocationDate { get; set; }
        }

        public class Result : SingleResponse<DateTime>
        {
        }
        public class Notification : INotification
        {
            public Allocation Allocation { get; set; }
            public DateTime OldAllocationDate { get; set; }
            public DateTime NewAllocationDate { get; set; }
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
                
                if (!await _accessControlService.HasAllocationAccess(request.AllocationId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.AllocationNotFound));
                }
                var allocation = await _writeDbContext.Allocations
                                                       .FirstOrDefaultAsync(x => x.AllocationId == request.AllocationId, cancellationToken: cancellationToken)
                                  ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                var oldDate = allocation.AllocationDate;
                allocation.SetAllocationDate(request.AllocationDate);

                await _writeDbContext.SaveChangesAsync(cancellationToken);

                _ = _mediator.Publish(new Notification()
                                      {
                                          Allocation = allocation,
                                          OldAllocationDate = oldDate,
                                          NewAllocationDate = allocation.AllocationDate
                                      }, cancellationToken);

                return new Result()
                       {
                           Data = allocation.AllocationDate
                       };
            }
        }
    }
}