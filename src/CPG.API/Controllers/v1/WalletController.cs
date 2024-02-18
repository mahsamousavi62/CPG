using CPG.Application.UseCases.NeoBankServices.Queries;
using CPG.Application.UseCases.NeoBankServices.ViewModels;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1;

/// <summary>
/// 
/// </summary>
public class WalletController : ApiBaseController
{
    /// <summary>
    /// GetUserDepositBalance From NeoBank
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("WalletInformation")]
    [ProducesResponseType(typeof(Result<Result<UserDepositBalanceResponse>>), 200)]
    public async Task<Result<UserDepositBalanceResponse>> GetWalletInformation()
    => await Mediator.Send(new GetUserDepositBalanceQuery());

}
