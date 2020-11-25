using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Budget.Command;
using raBudget.Application.Features.Budgets.Query;
using raBudget.Domain.Interfaces;

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

        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpGet("get-list")] public async Task<GetBudgetList.Response> GetBudgetList([FromQuery] GetBudgetList.Query query) => await _mediator.Send(query);

        [HttpPost("create")] public async Task<CreateBudget.Result> CreateBudget([FromBody] CreateBudget.Command command) => await _mediator.Send(command);
        [HttpPost("remove")] public async Task<RemoveBudget.Result> RemoveBudget([FromBody] RemoveBudget.Command command) => await _mediator.Send(command);
        [HttpPatch("update/name")] public async Task<UpdateBudgetName.Result> UpdateBudgetName([FromBody] UpdateBudgetName.Command command) => await _mediator.Send(command);
        [HttpPatch("update/starting-date")] public async Task<UpdateBudgetStartingDate.Result> UpdateBudgetStartingDate([FromBody] UpdateBudgetStartingDate.Command command) => await _mediator.Send(command);
    }
}