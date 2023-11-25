using System.Threading.Tasks;
using CPG.Application.UseCases.Auth.Commands.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1
{
    public class AuthController : ApiBaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginCommand command)
            => Ok(await Mediator.Send(command));
    }
}
