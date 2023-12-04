using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentRequestController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentRequestViewModel),200)]
        public async Task<ActionResult<PaymentRequestViewModel>> PaymentRequest([FromBody]CreatePaymentRequestViewModel model)
        => Ok(await Mediator.Send(new CreatePaymentRequestCommand(model)));
    }
}
