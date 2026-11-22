using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using raBudget.Application.Infrastructure;
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
	public class UpdateAllocationAmount
	{
		public class Command : IApplicationRequest<Result>
		{
			public AllocationId AllocationId { get; set; }
			public MoneyAmount Amount { get; set; }
		}

		public class Result : SingleResponse<MoneyAmount>
		{
		}
        public class Notification : UserNotification
        {
            public Allocation ReferenceAllocation { get; set; }
        }

		public class Handler : ICommandHandler<Command, Result>
		{
			private readonly IWriteDbContext _writeDbContext;
			private readonly AccessControlService _accessControlService;
            private readonly PostCommitEventPublisher _eventPublisher;

			public Handler(IWriteDbContext writeDbContext, AccessControlService accessControlService, PostCommitEventPublisher eventPublisher)
			{
				_writeDbContext = writeDbContext;
				_accessControlService = accessControlService;
                _eventPublisher = eventPublisher;
            }

			public async Task<Result> ExecuteAsync(Command request, CancellationToken cancellationToken)
			{
				if (!await _accessControlService.HasAllocationAccess(request.AllocationId))
				{
					throw new NotFoundException(Localization.For(() => ErrorMessages.AllocationNotFound));
				}

				var allocation = await _writeDbContext.Allocations
													  .FirstOrDefaultAsync(x => x.AllocationId == request.AllocationId, cancellationToken: cancellationToken)
								 ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

				allocation.SetAmount(request.Amount);

				await _writeDbContext.SaveChangesAsync(cancellationToken);

                _ = _eventPublisher.PublishAsync(new Notification() { ReferenceAllocation = allocation }, cancellationToken);

				return new Result()
				{
					Data = allocation.Amount
				};
			}
		}
	}
}