using System;
using System.Globalization;
using System.Threading.Tasks;
using FastEndpoints;
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
        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpPost("search")] public async Task<GetAllocationList.Result> SearchAllocations([FromBody] GetAllocationList.Query query) => await query.ExecuteAsync();
        [HttpPost("create")] public async Task<CreateAllocation.Result> CreateAllocation([FromBody] CreateAllocation.Command command) => await command.ExecuteAsync();
        [HttpPost("remove")] public async Task<RemoveAllocation.Result> RemoveAllocation([FromBody] RemoveAllocation.Command command) => await command.ExecuteAsync();
        [HttpPatch("update/description")] public async Task<UpdateAllocationDescription.Result> UpdateAllocationDescription([FromBody] UpdateAllocationDescription.Command command) => await command.ExecuteAsync();
        [HttpPatch("update/allocation-date")] public async Task<UpdateAllocationDate.Result> UpdateAllocationDate([FromBody] UpdateAllocationDate.Command command) => await command.ExecuteAsync();
        [HttpPatch("update/amount")] public async Task<UpdateAllocationAmount.Result> UpdateAllocationAmount([FromBody] UpdateAllocationAmount.Command command) => await command.ExecuteAsync();
        [HttpPatch("update/target-budget-category")] public async Task<UpdateAllocationTargetCategory.Result> UpdateAllocationCategory([FromBody] UpdateAllocationTargetCategory.Command command) => await command.ExecuteAsync();
        [HttpPatch("update/source-budget-category")] public async Task<UpdateAllocationSourceCategory.Result> UpdateAllocationCategory([FromBody] UpdateAllocationSourceCategory.Command command) => await command.ExecuteAsync();

        [HttpGet("get-dates-range")] public async Task<GetAllocationsDatesRange.Result> GetAllocationsDatesRange([FromQuery] GetAllocationsDatesRange.Query query) => await query.ExecuteAsync();

    }
}