using CPG.Application.UseCases.Users;
using CPG.Application.UseCases.Users.Commands;
using CPG.Application.UseCases.Users.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

namespace CPG.API.Controllers;

/// <summary>
/// 
/// </summary>
public class UsersController : ApiBaseController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("CreateUserProfile")]
    public async Task<IActionResult> CreateUserProfile(CreateUserCommnad command)
    {
        await Mediator.Send(command);
        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserViewModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCompanyUsers() => Ok(await Mediator.Send(new GetCompanyUsersQuery()));
}
