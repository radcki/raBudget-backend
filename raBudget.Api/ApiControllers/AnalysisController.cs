using System;
using System.Globalization;
using System.Threading.Tasks;
using MediatR;
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
        private readonly IMediator _mediator;

        public AnalysisController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpGet("get-transactions-timeline")] public async Task<GetTransactionsTimeline.Result> GetTransactionsTimeline([FromQuery] GetTransactionsTimeline.Query query) => await _mediator.Send(query);
    }
}