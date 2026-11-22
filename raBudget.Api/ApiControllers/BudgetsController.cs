using System;
using System.Globalization;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Budget.Command;
using raBudget.Application.Features.Budgets.Query;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Route("budget")]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpGet("get-list")] public async Task<GetBudgetList.Response> GetBudgetList([FromQuery] GetBudgetList.Query query) => await query.ExecuteAsync();
        [HttpGet("balance")] public async Task<GetBudgetBalance.Response> GetBudgetBalance([FromQuery] GetBudgetBalance.Query query) => await query.ExecuteAsync();

        [HttpPost("create")] public async Task<CreateBudget.Result> CreateBudget([FromBody] CreateBudget.Command command) => await command.ExecuteAsync();
        [HttpPost("remove")] public async Task<RemoveBudget.Result> RemoveBudget([FromBody] RemoveBudget.Command command) => await command.ExecuteAsync();
        [HttpPatch("update/name")] public async Task<UpdateBudgetName.Result> UpdateBudgetName([FromBody] UpdateBudgetName.Command command) => await command.ExecuteAsync();
        [HttpPatch("update/starting-date")] public async Task<UpdateBudgetStartingDate.Result> UpdateBudgetStartingDate([FromBody] UpdateBudgetStartingDate.Command command) => await command.ExecuteAsync();
    }
}