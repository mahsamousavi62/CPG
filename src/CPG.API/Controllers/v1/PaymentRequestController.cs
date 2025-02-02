using CPG.Application.UseCases.CharismaCard.Commands;
using CPG.Application.UseCases.CharismaCard.ViewModels;
using CPG.Application.UseCases.DirectDebit.Queries;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentReceipt.Commands;
using CPG.Application.UseCases.PaymentReceipt.Queries;
using CPG.Application.UseCases.PaymentReceipt.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Commands.CancelPaymentRequet;
using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;
using CPG.Application.UseCases.PaymentRequests.Queries.AnonymousStatus;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Infrastructure.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CPG.API.Controllers.v1;

public class PaymentRequestController : ApiBaseController
{   
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

    [AllowAnonymous]
    [HttpPost("CreateIPGJsonStr")]
    [ProducesResponseType(typeof(ResultData<PaymentTokenResponseViewModel>), 200)]
    public async Task<Result<PaymentTokenResponseViewModel>> GetAsanPardakhatPaymentTicket([FromBody] PaymentTokenViewModel paymentTicketRequest)
        => await Mediator.Send(new GetPaymentTokenCommand(paymentTicketRequest));

    [Authorize]
    [HttpPost("CreateWithdrawalRequest")]
    [ProducesResponseType(typeof(ResultData<bool>), 200)]
    public async Task<Result<bool>> CreateWithdrawalRequest([FromBody] WithdrawalRequestViewModel withdrawalRequest)
       => await Mediator.Send(new GetWithdrawalRequestQuery(withdrawalRequest));

    [AllowAnonymous]
    [HttpPost("CreatePaymentReceiptRequest")]
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

    [Authorize(Policy = AuthPolicies.Roles.AdminOrCompanyUser)]
    [HttpPost("PaymentReceiptTransactionVerify")]
    [ProducesResponseType(typeof(Result<string>), 200)]
    public async Task<Result<string>> PaymentReceiptTransactionVerify([Required] VerifyPaymentReceiptTransactionViewModel model)
        => await Mediator.Send(new VerifiyPaymentReceiptCommand(model));

    [Authorize]
    [HttpPost("CreateCharismaCardRequest")]
    [ProducesResponseType(typeof(Result<CharismaCardResponseViewModel>), 200)]
    public async Task<Result<CharismaCardResponseViewModel>> GetClientDirectDebit(CharismaCardRequsetViewModel model)
       => await Mediator.Send(new CreateCharismaCardTransactionCommand(model));
       
    [HttpGet("AnonymousStatus")]
    [ProducesResponseType(typeof(Result<AnonymousStatusResponseViewModel>), 200)]
    public async Task<Result<AnonymousStatusResponseViewModel>> AnonymousStatus([FromQuery] AnonymousStatusViewModel anonymousStatusRequest)
        => await Mediator.Send(new AnonymousStatusQuery(anonymousStatusRequest));
}
