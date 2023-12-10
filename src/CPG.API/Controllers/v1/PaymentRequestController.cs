using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;
using CPG.Application.UseCases.PaymentRequests.Queries;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CPG.API.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentRequestController : ApiBaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<PaymentRequestVm>), 200)]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetPaymentRequestQuery()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(PaymentRequestViewModel),200)]
        public async Task<ActionResult<PaymentRequestViewModel>> PaymentRequest([FromBody]CreatePaymentRequestViewModel model)
        => Ok(await Mediator.Send(new CreatePaymentRequestCommand(model)));
    }
}
