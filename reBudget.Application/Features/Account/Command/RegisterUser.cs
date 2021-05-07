using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using raBudget.Domain.Entities;
using raBudget.Domain.Interfaces;

namespace raBudget.Application.Features.Account.Command
{
    public class RegisterUser
    {
        public class Command : IRequest<Result>
        {
        }

        public class Result
        {
        }

        public class Success : INotification
        {
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IUserContext _userContext;

            public Handler(IUserContext userContext)
            {
                _userContext = userContext;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = new ApplicationUser()
                           {
                               UserId = _userContext.UserId
                           };
                //todo
                return new Result();
            }

        }
    }
}
