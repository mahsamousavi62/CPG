using CPG.Application.UseCases.Banks.Commands.ActivateBank;
using CPG.Application.UseCases.Banks.Commands.UpdateBank;
using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers;

//[Authorize]
public class BanksController : ApiBaseController
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BankViewModel>> GetBank(int id)
        => Ok(await Mediator.Send(new GetBankQuery(id)));

    [HttpGet("all")]
    public async Task<ActionResult<IReadOnlyCollection<BankViewModel>>> GetAllBanks()
        => Ok(await Mediator.Send(new GetAllBanksQuery()));

    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyCollection<BankViewModel>>> GetActiveBanks()
        => Ok(await Mediator.Send(new GetActiveBanksQuery()));

    [HttpPost("update/{bankId:int}/{ibanPrefix}")]
    public async Task<IActionResult> UpdateBank(UpdateBankCommand command)
    {
        await Mediator.Send(command);

        return Accepted();
    }

    [HttpPost("activate/{bankId:int}/{isActive:bool}")]
    public async Task<IActionResult> ActivateBank(int bankId, bool isActive)
    {
        await Mediator.Send(new ActivateBankCommand(bankId, isActive));

        return Accepted();
    }
}