using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using raBudget.Common.Resources;
using raBudget.Domain.Exceptions;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Services;
using RLib.Localization;

namespace raBudget.Application.Features.Budget.Command
{
    public class UpdateBudget
    {
        public class Command : IRequest<Result>
        {
            public int BudgetId { get; set; }
            public string Name { get; set; }
            public DateTime StartingDate { get; set; }
        }

        public class Result
        {
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IWriteDbContext _writeDbContext;
            private readonly IUserContext _userContext;
            private readonly AccessControlService _accessControlService;

            public Handler
            (
                IWriteDbContext writeDbContext,
                IUserContext userContext, 
                AccessControlService accessControlService)
            {
                _writeDbContext = writeDbContext;
                _userContext = userContext;
                _accessControlService = accessControlService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!await _accessControlService.HasBudgetAccessAsync(request.BudgetId))
                {
                    throw new NotFoundException(Localization.For(() => ErrorMessages.BudgetNotFound));
                }

                var entity = await _writeDbContext.Budgets.FirstOrDefaultAsync(x => x.BudgetId == new Domain.Entities.Budget.Id(request.BudgetId) 
                                                                                    && x.OwnerUserId == _userContext.UserId, cancellationToken);


                entity.SetName(request.Name);
                entity.SetStartingDate(request.StartingDate);
                await _writeDbContext.SaveChangesAsync(cancellationToken);
                return new Result();
            }
        }
    }
}