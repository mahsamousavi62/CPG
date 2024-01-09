using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Banks.Queries;

public class GetBankQuery(int bankId) : IRequest<Result<BankViewModel>>
{
    public int BankId { get; } = bankId;
}
