using CPG.Application.UseCases.PaymentRequests.ViewModels;
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
    /// <summary>
    /// Create User Profile
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost("CreateUserProfile")]
    [ProducesResponseType(typeof(Result<long>), (int)HttpStatusCode.OK)]
    public async Task<Result<long>> CreateUserProfile()
    {
        return await Mediator.Send(new CreateUserCommand());
    }

    /// <summary>
    /// Get User
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("GetUserProfile")]
    [ProducesResponseType(typeof(Result<UserViewModel>), (int)HttpStatusCode.OK)]
    public async Task<Result<UserViewModel>> Get()
    {
        return await Mediator.Send(new GetUserQuery());
    }

    /// <summary>
    /// Get user By PaymentCode
    /// </summary>
    /// <param name="paymentCode"></param>
    /// <returns></returns>
    [HttpGet("{paymentCode}")]
    [ProducesResponseType(typeof(Result<UserViewModel>), (int)HttpStatusCode.OK)]
    public async Task<Result<UserViewModel>> GetuserByPaymentCode(string paymentCode)
    {
        return await Mediator.Send(new GetUserByPaymentCodeQuery(paymentCode));
    }


    /// <summary>
    /// Get Company Users
    /// </summary>
    /// <returns></returns>
    [HttpGet("CompanyUsers")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<UserCompanyViewModel>>), (int)HttpStatusCode.OK)]
    public async Task<Result<IReadOnlyCollection<UserCompanyViewModel>>> GetCompanyUsers()
    {
        return await Mediator.Send(new GetCompanyUsersQuery());
    }
}
