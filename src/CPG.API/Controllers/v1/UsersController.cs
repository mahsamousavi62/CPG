using CPG.Application.UseCases.Users.Commands;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers;

public class UsersController : ApiBaseController
{
    [HttpPost("CreateUserProfile")]
    public async Task<IActionResult> CreateUserProfile(CreateUserCommnad command)
    {
        await Mediator.Send(command);
        return Ok();
    }
}
