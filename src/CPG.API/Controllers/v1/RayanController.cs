using CPG.Application.UseCases.CompanyIPGs.Queries;
using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers.v1
{
    [Route("")]
    public class RayanController : ApiBaseController
    {
        [HttpPost("p/b/{id}")]
        [ProducesResponseType(typeof(IReadOnlyCollection<CompanyIPGDataViewModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetDataFromRayan(string id)
        => Ok(id);
    }
}
