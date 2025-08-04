using CPG.Application.UseCases.CharismaCard.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CharismaCard.Queries;

public class GetCharismaCardBalanceQuery : IRequest<Result<CharismaCardBalanceViewModel>>
{
}