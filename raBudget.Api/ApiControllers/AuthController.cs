using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Account.Command;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")] public async Task<RegisterUser.Result> Register([FromBody] RegisterUser.Command command) => await _mediator.Send(command);
    }
}