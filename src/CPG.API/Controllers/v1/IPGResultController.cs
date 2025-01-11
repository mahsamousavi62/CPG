using CPG.Application.UseCases.Ipg.Queries;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.Users.Commands;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace CPG.API.Controllers.v1;

[Route("IPGResult")]
public class IPGResultController : ApiBaseController
{    
    [HttpPost("p/b/{id}")]
    public async Task<IActionResult> GetData([FromRoute] string id)
    {
        var response = await Mediator.Send(new CreateRedirectUrlCommnad(Request.Form, id));
        return Redirect(response.Data);
    }    
    
    [HttpPost("{trackId}")]
    [ProducesResponseType(typeof(Result<ValidateTokenResponseViewModel>), (int)HttpStatusCode.OK)]
    public async Task<Result<ValidateTokenResponseViewModel>> ValidateToken(string trackId,
                                                                            [AllowNull][FromBody] ValidateTokenRequestViewModel model)
    {
        var redirectUrlData = await Mediator.Send(new ValidateTokenQuery(model, trackId));
        return redirectUrlData;
    }
}
