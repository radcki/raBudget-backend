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
    [Route("Allocation")]
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

        [HttpGet("get-list")] public async Task<GetAllocationList.Result> GetAllocationList([FromQuery] GetAllocationList.Query query) => await _mediator.Send(query);
        [HttpPost("create")] public async Task<CreateAllocation.Result> CreateAllocation([FromBody] CreateAllocation.Command command) => await _mediator.Send(command);
        [HttpPost("remove")] public async Task<RemoveAllocation.Result> RemoveAllocation([FromBody] RemoveAllocation.Command command) => await _mediator.Send(command);
        [HttpPatch("update/description")] public async Task<UpdateAllocationDescription.Result> UpdateAllocationDescription([FromBody] UpdateAllocationDescription.Command command) => await _mediator.Send(command);
        [HttpPatch("update/Allocation-date")] public async Task<UpdateAllocationDate.Result> UpdateAllocationDate([FromBody] UpdateAllocationDate.Command command) => await _mediator.Send(command);
        [HttpPatch("update/amount")] public async Task<UpdateAllocationAmount.Result> UpdateAllocationAmount([FromBody] UpdateAllocationAmount.Command command) => await _mediator.Send(command);
        [HttpPatch("update/target-budget-category")] public async Task<UpdateAllocationTargetCategory.Result> UpdateAllocationCategory([FromBody] UpdateAllocationTargetCategory.Command command) => await _mediator.Send(command);
        [HttpPatch("update/source-budget-category")] public async Task<UpdateAllocationSourceCategory.Result> UpdateAllocationCategory([FromBody] UpdateAllocationSourceCategory.Command command) => await _mediator.Send(command);
        
    }
}