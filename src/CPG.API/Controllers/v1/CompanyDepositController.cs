using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
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
     => Ok(await Mediator.Send(new GetAllCompanyDepositQuery()));

        [HttpPost("GetAccountNumber")]
        public async Task<ActionResult<AccountNumberViewModel>> GetAccoutnNumber([FromBody] IbanViewModel model)
        {
            var result = await Mediator.Send(new GetAccountNumberQuery(model.Iban));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompanyDeposit(CreateCompanyDepositViewModel model)
        {
            var companyDepositId = await Mediator.Send(new CreateCompanyDepositCommand(model));

            return CreatedAtAction(nameof(CreateCompanyDeposit), new { id = companyDepositId }, new { companyDepositId });
        }

    }
}
