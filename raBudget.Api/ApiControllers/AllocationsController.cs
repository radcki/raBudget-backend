using System;
using System.Globalization;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Allocations.Command;
using raBudget.Application.Features.Allocations.Query;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Route("allocation")]
    [Authorize]
    public class AllocationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AllocationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpPost("search")] public async Task<GetAllocationList.Result> SearchAllocations([FromBody] GetAllocationList.Query query) => await _mediator.Send(query);
        [HttpPost("create")] public async Task<CreateAllocation.Result> CreateAllocation([FromBody] CreateAllocation.Command command) => await _mediator.Send(command);
        [HttpPost("remove")] public async Task<RemoveAllocation.Result> RemoveAllocation([FromBody] RemoveAllocation.Command command) => await _mediator.Send(command);
        [HttpPatch("update/description")] public async Task<UpdateAllocationDescription.Result> UpdateAllocationDescription([FromBody] UpdateAllocationDescription.Command command) => await _mediator.Send(command);
        [HttpPatch("update/allocation-date")] public async Task<UpdateAllocationDate.Result> UpdateAllocationDate([FromBody] UpdateAllocationDate.Command command) => await _mediator.Send(command);
        [HttpPatch("update/amount")] public async Task<UpdateAllocationAmount.Result> UpdateAllocationAmount([FromBody] UpdateAllocationAmount.Command command) => await _mediator.Send(command);
        [HttpPatch("update/target-budget-category")] public async Task<UpdateAllocationTargetCategory.Result> UpdateAllocationCategory([FromBody] UpdateAllocationTargetCategory.Command command) => await _mediator.Send(command);
        [HttpPatch("update/source-budget-category")] public async Task<UpdateAllocationSourceCategory.Result> UpdateAllocationCategory([FromBody] UpdateAllocationSourceCategory.Command command) => await _mediator.Send(command);

        [HttpGet("get-dates-range")] public async Task<GetAllocationsDatesRange.Result> GetAllocationsDatesRange([FromQuery] GetAllocationsDatesRange.Query query) => await _mediator.Send(query);

    }
}