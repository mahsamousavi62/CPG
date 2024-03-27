using AuthDemo.Security.Authorization;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;
using CPG.Application.UseCases.CompanyIPGs.Commands.UpdateCompanyIPG;
using CPG.Application.UseCases.CompanyIPGs.Queries;
using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers.v1;

public class CompanyIPGController : ApiBaseController
{
    [HttpGet("GetCompanyIPGs/{id:long}")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<CompanyIPGDataViewModel>>), (int)HttpStatusCode.OK)]
    public async Task<Result<IReadOnlyCollection<CompanyIPGDataViewModel>>> GetCompanyIPGs(long id)
    { 
        return await Mediator.Send(new GetCompanyIPGsQuery(id));
    }

    [HttpGet("GetActiveCompanyIPGs/{id:long}")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<CompanyIPGDataViewModel>>), (int)HttpStatusCode.OK)]
    public async Task<Result<IReadOnlyCollection<CompanyIPGDataViewModel>>> GetActiveCompanyIPGs(long id)
    { 
        return await Mediator.Send(new GetActiveCompanyIPGsQuery(id));
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Result<CompanyIPGViewModel>), (int)HttpStatusCode.OK)]
    public async Task<Result<CompanyIPGViewModel>> GetCompanyIPG(long id)
    { 
        return await Mediator.Send(new GetCompanyIPGQuery(id));
    }

    [HttpGet("GetCompanyIPGDeposits/{id:long}")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<CompanyDepositViewModel>>), (int)HttpStatusCode.OK)]
    public async Task<Result<IReadOnlyCollection<CompanyDepositViewModel>>> GetCompanyIPGDeposits(long id)
    { 
        return await Mediator.Send(new GetCompanyIPGDepositsQuery(id));
    }
    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPost]
    public async Task<Result<long>> CreateCompanyIPG(CreateCompanyIPGModel model)
    {
        var createIpgDepositViewModels = model.CompanyIPGDeposits.Select(t => new CreateCompanyIPGDepositViewModel { DepositId = t.DepositId, IsDefault = t.IsDefault }).ToList();
        var createViewModel = new CreateCompanyIPGViewModel(model.CompanyId, model.ProviderId, model.IPGTypeId, model.ProviderData, createIpgDepositViewModels);

        return await Mediator.Send(new CreateCompanyIPGCommand(createViewModel));
    }
    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPut]
    public async Task<Result<Unit>> UpdateCompanyIPG(UpdateCompanyIPGModel model)
    {
        var createIpgDepositViewModels = model.CompanyIPGDeposits.Select(t => new CreateCompanyIPGDepositViewModel { DepositId = t.DepositId, IsDefault = t.IsDefault }).ToList();
        var createViewModel = new UpdateCompanyIPGViewModel(model.Id,model.ProviderData, createIpgDepositViewModels);

        return await Mediator.Send(new UpdateCompanyIPGCommand(createViewModel));
    }
}