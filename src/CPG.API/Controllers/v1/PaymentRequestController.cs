using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;
using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;
using CPG.Application.UseCases.PaymentRequests.Queries;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.Ipg.Queries;
using Microsoft.AspNetCore.Http.HttpResults;


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
        [ProducesResponseType(typeof(Result<PaymentRequestResponseViewModel>), 200)]
        public async Task<Result<PaymentRequestResponseViewModel>> PaymentRequest([FromBody] CreatePaymentRequestViewModel model)
        { 
            return await Mediator.Send(new CreatePaymentRequestCommand(model));
        }

        [AllowAnonymous]
        [HttpPost("GetPaymentMethods")]
        [ProducesResponseType(typeof(Result<PaymentMethodsViewModel>), 200)]
        public async Task<Result<PaymentMethodsViewModel>> PaymentMethods([FromBody] GetPaymentMethodsViewModel model)
        {
            return await Mediator.Send(new GetPaymentMethodsCommand(model));
        }

        [HttpPost("CreateIPGJsonStr")]
        [Authorize]
        [ProducesResponseType(typeof(ResultData<PaymentTokenResponse>), 200)]
        public async Task<Result<PaymentTokenResponse>> GetAsanPardakhatPaymentTicket([FromBody] PaymentTokenViewModel paymentTicketRequest)
        { 
            return await Mediator.Send(new GetPaymentTokenCommand(paymentTicketRequest));
        }

        [HttpPost("GetPaymentTransactionInfo")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<ActionResult<ResultData<PaymentTokenResponse>>> GetPaymentTransactionInfo([FromBody] PaymentTransactionViewModel paymentTransactionRequest)
        => Ok(await Mediator.Send(new GetPaymentTransactionInfoQuery(paymentTransactionRequest)));
    }
}
