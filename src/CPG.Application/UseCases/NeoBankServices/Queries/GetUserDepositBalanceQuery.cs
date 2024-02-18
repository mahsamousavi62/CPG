using CPG.Application.UseCases.NeoBankServices.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using MediatR;

namespace CPG.Application.UseCases.NeoBankServices.Queries;

public class GetUserDepositBalanceQuery:IRequest<ResultData<UserDepositBalanceResponse>>
{

}
