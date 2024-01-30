using CPG.Application.UseCases.DirectDebit.Commands;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1
{
    public class DirectDebitController : ApiBaseController
    {
        [HttpPost("GetDirectDebitToken")]
        [Authorize]
        [ProducesResponseType(typeof(ResultData<bool>), 200)]
        public async Task<Result<bool>> GetToken([FromBody] GetTokenViewModel paymentTicketRequest)
            => await Mediator.Send(new GetTokenCommand(paymentTicketRequest));
    }
}
