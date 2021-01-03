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
    [Route("dictionary")]
    [Authorize]
    public class DictionariesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DictionariesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpGet("currencies")] public async Task<GetCurrencies.Result> GetCurrencies([FromQuery]GetCurrencies.Query query) => await _mediator.Send(query);
        [HttpGet("category-icons")] public async Task<GetBudgetCategoryIcons.Result> GetBudgetCategoryIcons([FromQuery] GetBudgetCategoryIcons.Query query) => await _mediator.Send(query);
    }
}