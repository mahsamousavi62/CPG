using CPG.Application.UseCases.Users.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1
{

    public class UsersController : ApiBaseController
    {
        [HttpPost("CreateUserProfile")]
        public async Task<IActionResult> CreateUserProfile(CreateUserCommnad command)
        {
            await Mediator.Send(command);
            return Ok();
        }
    }
}
