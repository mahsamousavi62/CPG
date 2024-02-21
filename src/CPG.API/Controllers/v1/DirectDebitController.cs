using CPG.Application.UseCases.DirectDebit.Commands;
using CPG.Application.UseCases.DirectDebit.Query;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers.v1;

[Authorize]
public class DirectDebitController : ApiBaseController
{
    [HttpGet("GetAvailableBankList")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<AvailableBankViewModel>>), 200)]
    public async Task<Result<IReadOnlyCollection<AvailableBankViewModel>>> GetAvailableBankList()
    {
        return await Mediator.Send(new GetAvailableBankListQuery());
    }

    [HttpPost("GetPlans")]
    [ProducesResponseType(typeof(Result<PlanViewModel>), 200)]
    public async Task<Result<PlanViewModel>> GetPlans([FromBody] GetDirectDebitPlansViewModel request)
        => await Mediator.Send(new GetDirectDebitPlansCommand(request));

    [HttpPost("GetUserPhoneNumbers")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<UserPhoneNumberViewModel>>), 200)]
    public async Task<Result<IReadOnlyCollection<UserPhoneNumberViewModel>>> GetUserPhoneNumbers([FromBody] GetUserPhoneNumbersViewModel request)
       => await Mediator.Send(new GetUserPhoneNumbersCommand(request));

    [HttpPost("ConfirmGrant")]
    [ProducesResponseType(typeof(Result<ConfirmGrantResponseViewModel>), (int)HttpStatusCode.OK)]
    public async Task<Result<ConfirmGrantResponseViewModel>> ConfirmGrant([FromBody] ConfirmGrantViewModel request)
    {
        var response = await Mediator.Send(new ConfirmGrantCommand(request));
        return response;
    }
}