using System.Threading.Tasks;
using CPG.Application.UseCases.CPGUsers.Commands.RegisterCPGUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers
{
    public class CPGUsersController : ApiBaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterCPGUser(RegisterCPGUserCommand command)
        {
            return Ok(await Mediator.Send(command));
            // TODO: CreatedAtAction(nameof(GetCPGUser), new { id = userId }, new { userId });
        }
    }
}
