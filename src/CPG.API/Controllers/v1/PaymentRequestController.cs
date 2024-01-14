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
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;

namespace CPG.API.Controllers.v1;

/// <summary>
/// 
/// </summary>
public class PaymentRequestController : ApiBaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<PaymentRequestViewModel>>), 200)]
    public async Task<Result<IReadOnlyCollection<PaymentRequestViewModel>>> Get()
    {
        return await Mediator.Send(new GetPaymentRequestQuery());
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
    [ProducesResponseType(typeof(ResultData<PaymentTokenResponseViewModel>), 200)]
    public async Task<Result<PaymentTokenResponseViewModel>> GetAsanPardakhatPaymentTicket([FromBody] PaymentTokenViewModel paymentTicketRequest)
    { 
        return await Mediator.Send(new GetPaymentTokenCommand(paymentTicketRequest));
    }

    [HttpPost("GetPaymentTransactionInfo")]
    [ProducesResponseType(typeof(Result<TransactionResultResponse>), 200)]
    public async Task<Result<TransactionResultResponse>> GetPaymentTransactionInfo([FromBody] PaymentTransactionViewModel paymentTransactionRequest)
    { 
       return await Mediator.Send(new GetPaymentTransactionInfoQuery(paymentTransactionRequest));
    }
}
