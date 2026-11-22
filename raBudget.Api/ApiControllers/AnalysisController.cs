using System;
using System.Globalization;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Analysis.Query;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Route("analysis")]
    [Authorize]
    public class AnalysisController : ControllerBase
    {
        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpGet("get-transactions-timeline")] public async Task<GetTransactionsTimeline.Result> GetTransactionsTimeline([FromQuery] GetTransactionsTimeline.Query query) => await query.ExecuteAsync();
    }
}