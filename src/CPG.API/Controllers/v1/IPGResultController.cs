using CPG.Application.UseCases.Ipg.Queries;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.IPGResult.ViewModels;
using CPG.Application.UseCases.Users.Commands;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace CPG.API.Controllers.v1;

[Authorize]
[Route("IPGResult")]
public class IPGResultController : ApiBaseController
{
    [AllowAnonymous]
    [HttpPost("p/b/{id}")]
    public async Task<IActionResult> GetData([FromRoute] string id)
    {
        var response = await Mediator.Send(new CreateRedirectUrlCommnad(Request.Form, id));
        return Redirect(response.Data);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> callback()
    {
        var response = await Mediator.Send(new CreateRedirectUrlCommnad(Request.Form, "ss"));
        return Redirect(response.Data);
    }

    [HttpPost("{trackId}")]
    [ProducesResponseType(typeof(Result<ValidateTokenResponseViewModel>), (int)HttpStatusCode.OK)]
    public async Task<Result<ValidateTokenResponseViewModel>> ValidateToken(string trackId, [AllowNull][FromBody] ValidateTokenRequestViewModel model)
    {
        var redirectUrlData = await Mediator.Send(new ValidateTokenQuery(model, trackId));
        return redirectUrlData;
    }

}

