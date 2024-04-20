using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;
using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Application.UseCases.PaymentRequests.Commands.CancelPaymentRequet;
using System.ComponentModel.DataAnnotations;
using CPG.Application.UseCases.DirectDebit.Queries;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Application.UseCases.PaymentReceipt.ViewModels;
using CPG.Application.UseCases.PaymentReceipt.Queries;
using CPG.Infrastructure.File;
using CPG.Application.UseCases.CharismaCard.Commands;
using CPG.Application.UseCases.CharismaCard.ViewModels;

namespace CPG.API.Controllers.v1;

public class PaymentRequestController : ApiBaseController
{

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

    [HttpPost("CreateWithdrawalRequest")]
    [Authorize]
    [ProducesResponseType(typeof(ResultData<bool>), 200)]
    public async Task<Result<bool>> CreateWithdrawalRequest([FromBody] WithdrawalRequestViewModel withdrawalRequest)
       => await Mediator.Send(new GetWithdrawalRequestQuery(withdrawalRequest));

    [HttpPost("CreatePaymentReceiptRequest")]
    [Authorize]
    [ProducesResponseType(typeof(ResultData<PaymentReceiptResponseViewModel>), 200)]
    public async Task<Result<PaymentReceiptResponseViewModel>> CreatePaymentReceiptRequest([FromForm] PaymentReceiptRequestViewModel paymentReceiptRequest)
    {
        CreatePaymentReceiptModel model = new(paymentReceiptRequest.PaymentRequestCode, new FormFileProxy(paymentReceiptRequest.File),
            paymentReceiptRequest.Iban, paymentReceiptRequest.ReceiptIdentifier, paymentReceiptRequest.CompanyDepositId,
            paymentReceiptRequest.SettlementDateTime, paymentReceiptRequest.Description);

        return await Mediator.Send(new AddPaymentReceiptQuery(model));
    }

    [HttpPost("GetPaymentTransactionInfo")]
    [ProducesResponseType(typeof(Result<TransactionResultResponse>), 200)]
    public async Task<Result<TransactionResultResponse>> GetPaymentTransactionInfo([FromBody] PaymentTransactionViewModel paymentTransactionRequest)
        => await Mediator.Send(new GetPaymentTransactionInfoQuery(paymentTransactionRequest));

    [Authorize]
    [HttpPost("TransactionDetail")]
    [ProducesResponseType(typeof(Result<TransactionDetailResponseViewModel>), 200)]
    public async Task<Result<TransactionDetailResponseViewModel>> TransactionDetail([Required] TransactionDetailRequestViewModel model)
        => await Mediator.Send(new TransactionDetailQuery(model));

    [HttpPost("TransactionVerify")]
    [ProducesResponseType(typeof(Result<VerifyTransactionResponseViewModel>), 200)]
    public async Task<Result<VerifyTransactionResponseViewModel>> TransactionVerify([Required] VerifyTransactionViewModel model)
        => await Mediator.Send(new VerifyTransactionQuery(model));

    [Authorize]
    [HttpPost("CreateCharismaCardRequest")]
    [ProducesResponseType(typeof(Result<CharismaCardResponseViewModel>), 200)]
    public async Task<Result<CharismaCardResponseViewModel>> GetClientDirectDebit(CharismaCardRequsetViewModel model)
       => await Mediator.Send(new CreateCharismaCardTransactionCommand(model));
}
