using CPG.Application.UseCases.Auth.Commands.Login;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers;

public class AuthController : ApiBaseController
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<Result<LoginCommandResponse>> Login(LoginCommand command)
    { 
        return await Mediator.Send(command);
    }
}