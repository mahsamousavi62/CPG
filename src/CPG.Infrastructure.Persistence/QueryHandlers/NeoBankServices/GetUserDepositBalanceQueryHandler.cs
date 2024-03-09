using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.NeoBankServices.Queries;
using CPG.Application.UseCases.NeoBankServices.ViewModels;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using MediatR;

namespace CPG.Infrastructure.Persistence.QueryHandlers.NeoBankServices;

public class GetUserDepositBalanceQueryHandler(INeoBankService neoBankService) : IRequestHandler<GetUserDepositBalanceQuery, Result<UserDepositBalanceViewModel>>
{
    private readonly INeoBankService neoBankService = neoBankService;

    public async Task<Result<UserDepositBalanceViewModel>> Handle(GetUserDepositBalanceQuery request, CancellationToken cancellationToken)
    {
        var response = await neoBankService.GetUserDepositBalance();

        return Result<UserDepositBalanceViewModel>.SuccessResult(new UserDepositBalanceViewModel
        {

            BalanceAmount = response.Data.Balance,
            CardNumber = response.Data.CardNumber,
            CustomerSureName = response.Data.CustomerLastName,
            Status = response.Data.DepositStatus,
            ExpirationDate = response.Data.ExpirationDate,
        });
    }
}
