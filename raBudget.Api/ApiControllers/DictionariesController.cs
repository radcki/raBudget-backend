using System;
using System.Globalization;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Dictionaries.Query;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Route("dictionary")]
    [Authorize]
    public class DictionariesController : ControllerBase
    {
        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpGet("currencies")] public async Task<GetCurrencies.Result> GetCurrencies([FromQuery]GetCurrencies.Query query) => await query.ExecuteAsync();
        [HttpGet("category-icons")] public async Task<GetBudgetCategoryIcons.Result> GetBudgetCategoryIcons([FromQuery] GetBudgetCategoryIcons.Query query) => await query.ExecuteAsync();
    }
}