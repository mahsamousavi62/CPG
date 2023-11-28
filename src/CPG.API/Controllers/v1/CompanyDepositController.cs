using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyDepositController : ApiBaseController
    {
        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(IReadOnlyCollection<CompanyDepositViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IReadOnlyCollection<CompanyDepositViewModel>>> GetAll()
     => Ok(await Mediator.Send(new GetAllCompanyQuery()));
    }
}
