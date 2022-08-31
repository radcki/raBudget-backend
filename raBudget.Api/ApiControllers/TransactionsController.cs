using System;
using System.Globalization;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using raBudget.Application.Features.Transactions.Command;
using raBudget.Application.Features.Transactions.Query;

namespace raBudget.Api.ApiControllers
{
    [ApiController]
    [Route("transaction")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("is-alive")]
        [AllowAnonymous]
        public string IsAlive()
        {
            return DateTime.UtcNow.ToString(CultureInfo.CurrentCulture);
        }

        [HttpPost("search")] public async Task<GetTransactionList.Result> SearchTransactions([FromBody] GetTransactionList.Query query) => await _mediator.Send(query);
        [HttpPost("create")] public async Task<CreateTransaction.Result> CreateTransaction([FromBody] CreateTransaction.Command command) => await _mediator.Send(command);
        [HttpPost("remove")] public async Task<RemoveTransaction.Result> RemoveTransaction([FromBody] RemoveTransaction.Command command) => await _mediator.Send(command);
        [HttpPatch("update/description")] public async Task<UpdateTransactionDescription.Result> UpdateTransactionDescription([FromBody] UpdateTransactionDescription.Command command) => await _mediator.Send(command);
        [HttpPatch("update/transaction-date")] public async Task<UpdateTransactionDate.Result> UpdateTransactionDate([FromBody] UpdateTransactionDate.Command command) => await _mediator.Send(command);
        [HttpPatch("update/amount")] public async Task<UpdateTransactionAmount.Result> UpdateTransactionAmount([FromBody] UpdateTransactionAmount.Command command) => await _mediator.Send(command);
        [HttpPatch("update/budget-category")] public async Task<UpdateTransactionCategory.Result> UpdateTransactionCategory([FromBody] UpdateTransactionCategory.Command command) => await _mediator.Send(command);
        

        [HttpPost("sub-transaction/add")] public async Task<AddSubTransaction.Result> AddSubTransaction([FromBody] AddSubTransaction.Command command) => await _mediator.Send(command);
        [HttpPost("sub-transaction/remove")] public async Task<RemoveSubTransaction.Result> RemoveSubTransaction([FromBody] RemoveSubTransaction.Command command) => await _mediator.Send(command);
        [HttpPatch("sub-transaction/update/description")] public async Task<UpdateSubTransactionDescription.Result> UpdateSubTransactionDescription([FromBody] UpdateSubTransactionDescription.Command command) => await _mediator.Send(command);
        [HttpPatch("sub-transaction/update/transaction-date")] public async Task<UpdateSubTransactionDate.Result> UpdateSubTransactionDateTime([FromBody] UpdateSubTransactionDate.Command command) => await _mediator.Send(command);
        [HttpPatch("sub-transaction/update/amount")] public async Task<UpdateSubTransactionAmount.Result> UpdateSubTransactionAmount([FromBody] UpdateSubTransactionAmount.Command command) => await _mediator.Send(command);
        
        [HttpGet("get-dates-range")] public async Task<GetTransactionsDatesRange.Result> GetTransactionsDatesRange([FromQuery] GetTransactionsDatesRange.Query query) => await _mediator.Send(query);
    }
}