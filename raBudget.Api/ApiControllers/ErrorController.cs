using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Budget.Command;
using raBudget.Application.Features.Dictionaries.Query;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Authorize]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error() => Problem();
    }
}