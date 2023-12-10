using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using HotChocolate.Execution.Processing;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers.v1
{

    public class CompanyDepositController : ApiBaseController
    {
        [HttpGet("GetAll")]
        [ProducesResponseType(typeof(IReadOnlyCollection<CompanyDepositViewModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IReadOnlyCollection<CompanyDepositViewModel>>> GetAll()
     => Ok(await Mediator.Send(new GetAllCompanyDepositQuery()));


        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(CompanyDepositViewModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CompanyDepositViewModel>> Get(long id)
            => Ok(await Mediator.Send(new GetCompanyDepositQuery(id)));




        [HttpPost("GetAccountNumber")]
        [ProducesResponseType(typeof(ResultData<AccountNumberViewModel>), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<ResultData<AccountNumberViewModel>>> GetAccoutnNumber([FromBody] IbanViewModel model)
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
