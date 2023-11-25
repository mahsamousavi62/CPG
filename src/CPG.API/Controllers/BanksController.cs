using CPG.Application.UseCases.Banks.Commands.CreateBank;
using CPG.Application.UseCases.Banks.Commands.DeleteBank;
using CPG.Application.UseCases.Banks.Queries;
using CPG.Application.UseCases.Banks.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers;

[Authorize]
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

    [HttpPost]
    public async Task<IActionResult> CreateBank(CreateBankCommand command)
    {
        var bankId = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetBank), new { id = bankId }, new { bankId });
    }

    [HttpPost("{bookId}/delete")]
    public async Task<IActionResult> DeleteBank(int bankId)
    {
        await Mediator.Send(new DeleteBankCommand(bankId));

        return Accepted();
    }

}
