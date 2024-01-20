using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;
using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;
using CPG.Application.UseCases.PaymentRequests.Queries;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Application.UseCases.PaymentRequests.Commands.CancelPaymentRequet;
using System.ComponentModel.DataAnnotations;

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
    => await Mediator.Send(new CreatePaymentRequestCommand(model));

    [AllowAnonymous]
    [HttpPost("GetPaymentMethods")]
    [ProducesResponseType(typeof(Result<PaymentMethodsViewModel>), 200)]
    public async Task<Result<PaymentMethodsViewModel>> PaymentMethods([FromBody] GetPaymentMethodsViewModel model)
    => await Mediator.Send(new GetPaymentMethodsCommand(model));


    [AllowAnonymous]
    [HttpPost("CancelPaymentRequest")]
    [ProducesResponseType(typeof(Result<CancelPaymentRequestResponseViewModel>), 200)]
    public async Task<Result<CancelPaymentRequestResponseViewModel>> CancelPaymentRequest([FromBody] CancelPaymentRequestViewModel model)
        => await Mediator.Send(new CancelPaymentRequestCommand(model));

    [HttpPost("CreateIPGJsonStr")]
    [Authorize]
    [ProducesResponseType(typeof(ResultData<PaymentTokenResponseViewModel>), 200)]
    public async Task<Result<PaymentTokenResponseViewModel>> GetAsanPardakhatPaymentTicket([FromBody] PaymentTokenViewModel paymentTicketRequest)
        => await Mediator.Send(new GetPaymentTokenCommand(paymentTicketRequest));

    [HttpPost("GetPaymentTransactionInfo")]
    [ProducesResponseType(typeof(Result<TransactionResultResponse>), 200)]
    public async Task<Result<TransactionResultResponse>> GetPaymentTransactionInfo([FromBody] PaymentTransactionViewModel paymentTransactionRequest)
    {
        return await Mediator.Send(new GetPaymentTransactionInfoQuery(paymentTransactionRequest));
    }

    [HttpPost("TransactionDetail")]
    [ProducesResponseType(typeof(Result<TransactionDetailResponseViewModel>), 200)]
    public async Task<Result<TransactionDetailResponseViewModel>> TransactionDetail([Required] TransactionDetailRequestViewModel model)
    {
        return await Mediator.Send(new TransactionDetailQuery(model));
    }

    [HttpPost("TransactionVerify")]
    [ProducesResponseType(typeof(Result<VerifyTransactionResponseViewModel>), 200)]
    public async Task<Result<VerifyTransactionResponseViewModel>> TransactionVerify([Required] VerifyTransactionViewModel model)
    {
        return await Mediator.Send(new VerifyTransactionQuery(model));
    }
}
