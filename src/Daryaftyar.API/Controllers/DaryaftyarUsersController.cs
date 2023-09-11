using System.Threading.Tasks;
using Daryaftyar.Application.UseCases.DaryaftyarUsers.Commands.RegisterDaryaftyarUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daryaftyar.API.Controllers
{
    public class DaryaftyarUsersController : ApiBaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterDaryaftyarUser(RegisterDaryaftyarUserCommand command)
        {
            return Ok(await Mediator.Send(command));
            // TODO: CreatedAtAction(nameof(GetDaryaftyarUser), new { id = userId }, new { userId });
        }
    }
}
