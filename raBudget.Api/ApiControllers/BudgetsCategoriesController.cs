using System;
using System.Globalization;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Budget.Command;
using raBudget.Application.Features.BudgetCategories.Command;
using raBudget.Application.Features.BudgetCategories.Query;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Route("budget-categories")]
    [Authorize]
    public class BudgetCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BudgetCategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpGet("")] public async Task<GetBudgetCategoryList.Result> GetBudgetCategoryList([FromQuery] GetBudgetCategoryList.Query query) => await _mediator.Send(query);
        [HttpGet("get-budgeted-amounts-summary")] public async Task<GetBudgetedAmountsSummary.Result> GetBudgetedAmountsSummary([FromQuery] GetBudgetedAmountsSummary.Query query) => await _mediator.Send(query);
        
        [HttpPost("create")] public async Task<CreateBudgetCategory.Result> CreateBudgetCategory([FromBody] CreateBudgetCategory.Command command) => await _mediator.Send(command);
        [HttpPost("remove")] public async Task<RemoveBudgetCategory.Result> RemoveBudget([FromBody] RemoveBudgetCategory.Command command) => await _mediator.Send(command);
        [HttpPatch("update/name")] public async Task<UpdateBudgetCategoryName.Result> UpdateBudgetCategoryName([FromBody] UpdateBudgetCategoryName.Command command) => await _mediator.Send(command);
        [HttpPatch("update/icon")] public async Task<UpdateBudgetCategoryIcon.Result> UpdateBudgetCategoryIcon([FromBody] UpdateBudgetCategoryIcon.Command command) => await _mediator.Send(command);
        [HttpPatch("update/name")] public async Task<UpdateBudgetCategoryVisibility.Result> UpdateBudgetCategoryVisibility([FromBody] UpdateBudgetCategoryVisibility.Command command) => await _mediator.Send(command);
        [HttpPatch("move-up")] public async Task<MoveBudgetCategoryUp.Result> MoveBudgetCategoryUp([FromBody] MoveBudgetCategoryUp.Command command) => await _mediator.Send(command);
        [HttpPatch("move-down")] public async Task<MoveBudgetCategoryDown.Result> MoveBudgetCategoryDown([FromBody] MoveBudgetCategoryDown.Command command) => await _mediator.Send(command);

        [HttpPost("get-balance")] public async Task<GetBudgetCategoryBalance.Result> GetBudgetCategoryBalance([FromBody] GetBudgetCategoryBalance.Query query) => await _mediator.Send(query);
        [HttpPost("get-current-balance")] public async Task<GetCurrentBudgetCategorySummary.Result> GetCurrentBudgetCategorySummary([FromBody] GetCurrentBudgetCategorySummary.Query query) => await _mediator.Send(query);

        
        [HttpPost("budgeted-amount/add")] public async Task<AddBudgetedAmount.Result> AddBudgetedAmount([FromBody] AddBudgetedAmount.Command command) => await _mediator.Send(command);
        [HttpPost("budgeted-amount/remove")] public async Task<RemoveBudgetedAmount.Result> RemoveBudgetedAmount([FromBody] RemoveBudgetedAmount.Command command) => await _mediator.Send(command);
        [HttpPatch("budgeted-amount/update/valid-from")] public async Task<UpdateBudgetedAmountValidFrom.Result> UpdateBudgetedAmountValidFrom([FromBody] UpdateBudgetedAmountValidFrom.Command command) => await _mediator.Send(command);
        [HttpPatch("budgeted-amount/update/amount")] public async Task<UpdateBudgetedAmountAmount.Result> UpdateBudgetedAmountAmount([FromBody] UpdateBudgetedAmountAmount.Command command) => await _mediator.Send(command);

    }

}