using CPG.Application.UseCases.Banks.Commands.ActivateBank;
using CPG.Application.UseCases.Banks.Commands.UpdateBank;
using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers;

//[Authorize]
public class BanksController : ApiBaseController
{
    [HttpGet("{id:int}")]
    public async Task<Result<BankViewModel>> GetBank(int id)
    { 
        return await Mediator.Send(new GetBankQuery(id));
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpGet("all")]
    public async Task<Result<IReadOnlyCollection<BankViewModel>>> GetAllBanks()
    { 
        return await Mediator.Send(new GetAllBanksQuery());
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpGet("active")]
    public async Task<Result<IReadOnlyCollection<BankViewModel>>> GetActiveBanks()
    { 
        return await Mediator.Send(new GetActiveBanksQuery());
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPut("update")]
    public async Task<Result<bool>> UpdateBank([FromForm] UpdateBankModel model)
    {
        UpdateBankViewModel updatebankViewModel = new(model.Id,model.Name,
            new FormFileProxy(model.Logo),model.IbanPrefix,model.HasDirectDebitFeature,model.DirectDebitSetting);

        return await Mediator.Send(new UpdateBankCommand(updatebankViewModel));
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPost("activate/{bankId:int}/{isActive:bool}")]
    public async Task<Result<bool>> ActivateBank(int bankId, bool isActive)
    {
        return await Mediator.Send(new ActivateBankCommand(bankId, isActive));
    }
}