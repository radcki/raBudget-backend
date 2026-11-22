using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Authorize]
    public class ErrorController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        public IActionResult Error() => Problem();
    }
}
