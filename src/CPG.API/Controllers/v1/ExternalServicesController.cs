using CPG.Application.Shared;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;
using CPG.Application.UseCases.PaymentRequests.Queries.Report;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.Ipg.Queries;
using System.ComponentModel.DataAnnotations;

namespace CPG.API.Controllers.v1;

public class ExternalServicesController : ApiBaseController
{
    [HttpPost("PaymentRequest")]
    [ProducesResponseType(typeof(Result<PaymentRequestResponseViewModel>), 200)]
    public async Task<Result<PaymentRequestResponseViewModel>> PaymentRequest([FromBody] CreatePaymentRequestViewModel model)
        => await Mediator.Send(new CreatePaymentRequestCommand(model));
       
    [Authorize]
    [ProducesResponseType(typeof(Result<CancelPaymentRequestResponseViewModel>), 200)]
    public async Task<Result<CancelPaymentRequestResponseViewModel>> CancelPaymentRequest([FromBody] CancelPaymentRequestViewModel model)
        => await Mediator.Send(new CancelPaymentRequestCommand(model));

    [Authorize]
    [HttpPost("TransactionDetail")]
    [ProducesResponseType(typeof(Result<TransactionDetailResponseViewModel>), 200)]
    public async Task<Result<TransactionDetailResponseViewModel>> TransactionDetail([Required] TransactionDetailRequestViewModel model)
        => await Mediator.Send(new TransactionDetailQuery(model));

    [Authorize]
    [HttpPost("TransactionVerify")]
    [ProducesResponseType(typeof(Result<VerifyTransactionResponseViewModel>), 200)]
    public async Task<Result<VerifyTransactionResponseViewModel>> TransactionVerify([Required] VerifyTransactionViewModel model)
        => await Mediator.Send(new VerifyTransactionQuery(model));

    [Authorize]
    [HttpGet("PaymentRequestReport")]
    [ProducesResponseType(typeof(Result<PagedList<PaymentRequestReportViewModel>>), 200)]
    public async Task<Result<PagedList<PaymentRequestReportViewModel>>> GetPaymentRequestsReport
        ([FromQuery] PaymentFilter searchTerm, [FromQuery] PagedFilter pagedFilter)
      => await Mediator.Send(new GetPaymentRequestsQuery(searchTerm, pagedFilter));
}
