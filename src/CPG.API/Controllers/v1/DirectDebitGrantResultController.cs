using CPG.Application.UseCases.DirectDebit.Queries;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace CPG.API.Controllers.v1
{

    [Authorize]
    [Route("DDGrantResult")]
    public class DirectDebitGrantResultController: ApiBaseController
    {
        [HttpPost("{trackId}")]
        [ProducesResponseType(typeof(Result<ValidateGrantResponseViewModel>), (int)HttpStatusCode.OK)]
        public async Task<Result<ValidateGrantResponseViewModel>> ValidateGrant(string trackId,
                                                                            [AllowNull][FromBody] ValidateGrantRequestViewModel model)
        {
            var redirectUrlData = await Mediator.Send(new ValidateGrantQuery(model, trackId));
            return redirectUrlData;
        }
    }
}
