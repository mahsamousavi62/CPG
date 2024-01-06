using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Application.UseCases.Users;
using CPG.Application.UseCases.Users.Commands;
using CPG.Application.UseCases.Users.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers;

/// <summary>
/// 
/// </summary>
public class UsersController : ApiBaseController
{
    [Authorize]
    [HttpPost("CreateUserProfile")]
    public async Task<IActionResult> CreateUserProfile()
    {
        await Mediator.Send(new CreateUserCommnad());
        return Ok();
    }
   
    [Authorize]
    [HttpGet("GetUserProfile")]
    [ProducesResponseType(typeof(Result<UserViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CompanyDepositViewModel>> Get()
              => Ok(await Mediator.Send(new GetUserQuery()));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("CompanyUsers")]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserCompanyViewModel>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCompanyUsers() => Ok(await Mediator.Send(new GetCompanyUsersQuery()));
}
