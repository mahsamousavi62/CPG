using CPG.Application.UseCases.DirectDebit.Commands;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Vandar.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1
{
    [ApiController]
    [Route("vandar")]
    public class VandarController : ApiBaseController
    {
        [HttpPost]
        [Route("dd-withdrawal-webhook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] WithdrawalWebhookRequest webhookData)
        {
            if (webhookData is null)
                return BadRequest();
            
            var data = await Mediator.Send(new SetVandarWithdrawalDataCommand(webhookData));
            if (data.IsSuccess)
                return Ok();

            return Problem();
        }
    }
}
