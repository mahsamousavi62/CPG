using CPG.Application.UseCases.Banks.Commands.ActivateBank;
using CPG.Application.UseCases.Banks.Commands.UpdateBank;
using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Domain.SharedKernel;
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

    [HttpGet("all")]
    public async Task<Result<IReadOnlyCollection<BankViewModel>>> GetAllBanks()
    { 
        return await Mediator.Send(new GetAllBanksQuery());
    }

    [HttpGet("active")]
    public async Task<Result<IReadOnlyCollection<BankViewModel>>> GetActiveBanks()
    { 
        return await Mediator.Send(new GetActiveBanksQuery());
    }

    [HttpPut("update")]
    public async Task<Result<bool>> UpdateBank([FromForm]UpdateBankViewModel model)
    {
        return await Mediator.Send(new UpdateBankCommand(model));
    }

    [HttpPost("activate/{bankId:int}/{isActive:bool}")]
    public async Task<Result<bool>> ActivateBank(int bankId, bool isActive)
    {
        return await Mediator.Send(new ActivateBankCommand(bankId, isActive));
    }
}