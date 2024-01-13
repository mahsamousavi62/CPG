using CPG.Application.UseCases.Ipg.Queries;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.Users.Commands;
using CPG.Domain.SharedKernel;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CPG.API.Controllers.v1;

[Authorize]
[Route("IPGResult")]
public class IPGResultController : ApiBaseController
{
    [AllowAnonymous]
    [HttpPost("p/b/{id}")]
    public async Task<IActionResult> GetData(string id, [FromForm]CreateRedirectUrlCommand command)
    {
        command = command with { Id = id };

        var response = await Mediator.Send(command);
        return Redirect(response.Data);
    }

    [HttpPost("TransactionDetail")]
    [ProducesResponseType(typeof(Result<TransactionDetailResponseViewModel>), 200)]
    public async Task<Result<TransactionDetailResponseViewModel>> GetTransactionDetail([Required] TransactionDetailRequestViewModel model)
    {
        return await Mediator.Send(new TransactionDetailQuery(model));
    }

    [HttpPost("TransactionVerify")]
    [ProducesResponseType(typeof(Result<VerifyTransactionResponseViewModel>), 200)]
    public async Task<Result<VerifyTransactionResponseViewModel>> VerifyTransaction([Required] VerifyTransactionViewModel model) {
        return await Mediator.Send(new VerifyTransactionQuery(model));
    }

    [HttpPost("{trackId}")]
    [ProducesResponseType(typeof(Result<ValidateTokenResponseViewModel>), (int)HttpStatusCode.OK)]
    public async Task<Result<ValidateTokenResponseViewModel>> ValidateToken(string trackId)
    {
        var redirectUrlData = await Mediator.Send(new ValidateTokenQuery(new ValidateTokenRequestViewModel { TrackId = trackId }));
        return redirectUrlData;
    }
}

