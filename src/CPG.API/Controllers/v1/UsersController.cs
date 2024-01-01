using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Application.UseCases.Users;
using CPG.Application.UseCases.Users.Commands;
using CPG.Application.UseCases.Users.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("{idpId}")]
    [ProducesResponseType(typeof(UserViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CompanyDepositViewModel>> Get(string idpId)
              => Ok(await Mediator.Send(new GetUserQuery(idpId)));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("CompanyUsers")]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserCompanyViewModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCompanyUsers() => Ok(await Mediator.Send(new GetCompanyUsersQuery()));
}
