using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.NeoBankServices.Queries;
using CPG.Application.UseCases.NeoBankServices.ViewModels;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using MediatR;

namespace CPG.Infrastructure.Persistence.QueryHandlers.NeoBankServices;

public class GetUserDepositBalanceQueryHandler(INeoBankService neoBankService) : IRequestHandler<GetUserDepositBalanceQuery, Result<UserDepositBalanceResponse>>
{
    private readonly INeoBankService neoBankService = neoBankService;

    public async Task<Result<UserDepositBalanceResponse>> Handle(GetUserDepositBalanceQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await neoBankService.GetUserDepositBalance();
            return Result<UserDepositBalanceResponse>.SuccessResult(result.Data);

        }
        catch (DomainException exc)
        {
            return Result<UserDepositBalanceResponse>.Failure(new Error(exc.Code, exc.Message));
        }
        
        catch (Exception exc)
        {
            return Result<UserDepositBalanceResponse>.Failure(new Error(exc.Source, exc.Message));
        }

    }
}
