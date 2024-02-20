using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;
using CPG.Application.UseCases.CompanyDeposits.Commands.SetAsDefaultForDD;
using CPG.Application.UseCases.CompanyDeposits.Commands.UpdateCompanyDeposit;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers.v1;

/// <summary>
/// 
/// </summary>
public class CompanyDepositController : ApiBaseController
{
    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<CompanyDepositViewModel>>), (int)HttpStatusCode.OK)]
    public async Task<Result<IReadOnlyCollection<CompanyDepositViewModel>>> GetAll()
    { 
        return await Mediator.Send(new GetAllCompanyDepositQuery());
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Result<CompanyDepositViewModel>), (int)HttpStatusCode.OK)]
    public async Task<Result<CompanyDepositViewModel>> GetById(long id)
    { 
        return await Mediator.Send(new GetCompanyDepositQuery(id));
    }

    [HttpGet("Company/{companyId:long}")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<CompanyDepositViewModel>>), (int)HttpStatusCode.OK)]
    public async Task<Result<IReadOnlyCollection<CompanyDepositViewModel>>> GetByCompanyId(long companyId)
    { 
        return await Mediator.Send(new GetCompanyDepositsByCompanyIdQuery(companyId));
    }

    [HttpPost("GetAccountNumber")]
    [ProducesResponseType(typeof(Result<AccountNumberResponse>), (int)HttpStatusCode.OK)]
    public async Task<Result<AccountNumberResponse>> GetAccoutnNumber([FromBody] IbanViewModel model)
    {
        return await Mediator.Send(new GetAccountNumberQuery(model.Iban));        
    }

    [HttpPost]
    public async Task<Result<long>> CreateCompanyDeposit(CreateCompanyDepositViewModel model)
    {
        return await Mediator.Send(new CreateCompanyDepositCommand(model));
    }

    [HttpPut]
    public async Task<Result<Unit>> UdpateCompanyDeposit(UpdateCompanyDepositViewModel model)
        => await Mediator.Send(new UpdateCompanyDepositCommand(model));
    
    [HttpPost("SetAsDefaultForDirecetDebit")]
    public async Task<Result<bool>> SetAsDefaultForDirecetDebit(SetAsDefaultForDDViewModel model)
    {
        return await Mediator.Send(new SetAsDefaultForDDCommand(model));
    }
}
