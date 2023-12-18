using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;
using CPG.Application.UseCases.CompanyIPGs.Queries;
using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers.v1;

public class CompanyIPGController : ApiBaseController
{
    [HttpGet("GetCompanyIPGs/{id:long}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CompanyIPGDataViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<CompanyIPGDataViewModel>>> GetCompanyIPGs(long id)
     => Ok(await Mediator.Send(new GetCompanyIPGsQuery(id)));

    [HttpGet("GetActiveCompanyIPGs/{id:long}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CompanyIPGDataViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<CompanyIPGDataViewModel>>> GetActiveCompanyIPGs(long id)
     => Ok(await Mediator.Send(new GetActiveCompanyIPGsQuery(id)));

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CompanyIPGViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CompanyIPGViewModel>> GetCompanyIPG(long id)
        => Ok(await Mediator.Send(new GetCompanyIPGQuery(id)));

    [HttpGet("GetCompanyIPGDeposits/{id:long}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CompanyDepositViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<CompanyDepositViewModel>>> GetCompanyIPGDeposits(long id)
        => Ok(await Mediator.Send(new GetCompanyIPGDepositsQuery(id)));

    [HttpPost]
    public async Task<IActionResult> CreateCompanyIPG(CreateCompanyIPGModel model)
    {
        var createIpgDepositViewModels = model.CompanyIPGDeposits.Select(t => new CreateCompanyIPGDepositViewModel { DepositId = t.DepositId, IsDefault = t.IsDefault }).ToList();
        var createViewModel = new CreateCompanyIPGViewModel(model.CompanyId, model.ProviderId, model.IPGTypeId, model.ProviderData, model.VerificationTimeLimit, createIpgDepositViewModels);

        var companyIpgId = await Mediator.Send(new CreateCompanyIPGCommand(createViewModel));

        return CreatedAtAction(nameof(CreateCompanyIPG), new { id = companyIpgId }, new { companyIpgId });
    }
}