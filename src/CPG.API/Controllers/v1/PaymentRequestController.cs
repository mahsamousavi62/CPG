using CPG.Application.UseCases.Ipg.Queries;
using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;
using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;
using CPG.Application.UseCases.PaymentRequests.Queries;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel;
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
        [ProducesResponseType(typeof(IReadOnlyCollection<PaymentRequestViewModel>), 200)]
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
        [ProducesResponseType(typeof(PaymentRequestResponseViewModel),200)]
        public async Task<ActionResult<PaymentRequestResponseViewModel>> PaymentRequest([FromBody]CreatePaymentRequestViewModel model)
        => Ok(await Mediator.Send(new CreatePaymentRequestCommand(model)));

        [AllowAnonymous]
        [HttpPost("GetPaymentMethods")]
        [ProducesResponseType(typeof(PaymentMethodsViewModel), 200)]
        public async Task<ActionResult<PaymentMethodsViewModel>> PaymentMethods([FromBody] GetPaymentMethodsViewModel model)
        => Ok(await Mediator.Send(new GetPaymentMethodsCommand(model)));

        [HttpPost("GetPaymentTicketFromAsanPardakhat")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<ActionResult<ResultData<PaymentTicketResponse>>> GetAsanPardakhatPaymentTicket([FromBody]PaymentTicketRequest paymentTicketRequest)
        {
            return Ok(await Mediator.Send(new GetPaymentTicketQuery(paymentTicketRequest)));
        }
    }
}
