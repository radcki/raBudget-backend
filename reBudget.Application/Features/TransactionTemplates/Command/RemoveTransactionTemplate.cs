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

namespace raBudget.Application.Features.TransactionTemplates.Command
{
    public class RemoveTransactionTemplate
    {
        public class Command : IRequest<Result>
        {
            public TransactionTemplateId TransactionTemplateId { get; set; }
        }

        public class Result : BaseResponse
        {
        }

        public class Notification : INotification
		{
			public TransactionTemplate TransactionTemplate { get; set; }
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
                var transactionTemplate = await _writeDbContext.TransactionTemplates
                                                       .FirstOrDefaultAsync(x => x.TransactionTemplateId == request.TransactionTemplateId, cancellationToken: cancellationToken)
                                  ?? throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));

                if (!await _accessControlService.HasTransactionTemplateAccess(transactionTemplate.TransactionTemplateId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.TransactionNotFound));
                }

                transactionTemplate.SoftDelete();

                await _writeDbContext.SaveChangesAsync(cancellationToken);


                _ = _mediator.Publish(new Notification()
                                      {
                                          TransactionTemplate = transactionTemplate
                                      }, cancellationToken);

                return new Result() { };
            }
        }
    }
}