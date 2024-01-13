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
    [ProducesResponseType(typeof(TransactionDetailResponseViewModel), 200)]
    public async Task<IActionResult> GetTransactionDetail([Required] TransactionDetailRequestViewModel model)
        => Ok(await Mediator.Send(new TransactionDetailQuery(model)));

    [HttpPost("TransactionVerify")]
    [ProducesResponseType(typeof(VerifyTransactionResponseViewModel), 200)]
    public async Task<IActionResult> VerifyTransaction([Required] VerifyTransactionViewModel model)
        => Ok(await Mediator.Send(new VerifyTransactionQuery(model)));

    [HttpPost("{trackId}")]
    [ProducesResponseType(typeof(ValidateTokenResponseViewModel), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ValidateToken(string trackId)
    {
        var redirectUrlData = await Mediator.Send(new ValidateTokenQuery(new ValidateTokenRequestViewModel { TrackId = trackId }));
        return Ok(redirectUrlData.Data);
    }
}

