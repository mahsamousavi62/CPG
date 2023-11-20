using CPG.Application.UseCases.Users.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers
{
    
    public class UsersController : ApiBaseController
    {
        [HttpPost("CreateUserProfile")]
        public async Task<IActionResult> CreateUserProfile(CreateUserCommnad command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
