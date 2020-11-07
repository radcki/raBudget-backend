using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;

namespace raBudget.Application.Features.Budget.Query
{
    public class GetBudgetList
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public List<BudgetDto> Data { get; set; }
            public int Total { get; set; }
        }

        public class BudgetDto
        {
            public int BudgetId { get; set; }
            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly IWriteDbContext _db;
            private readonly IUserContext _userContext;

            public Handler(IWriteDbContext db, IUserContext userContext)
            {
                _db = db;
                _userContext = userContext;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = _userContext.UserId;
                return new Result(){Data = new List<BudgetDto>(), Total = 0};
            }

        }
    }
}
