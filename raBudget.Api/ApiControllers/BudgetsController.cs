using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Account.Command;
using raBudget.Application.Features.Budget.Query;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Route("api/budget")]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BudgetsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-budgets-list")] public async Task<GetBudgetList.Result> Register([FromQuery] GetBudgetList.Query query) => await _mediator.Send(query);
    }
}