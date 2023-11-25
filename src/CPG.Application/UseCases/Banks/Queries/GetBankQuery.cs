using CPG.Application.UseCases.Banks.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Banks.Queries;

public class GetBankQuery(int bankId) : IRequest<BankViewModel>
{
    public int BankId { get; } = bankId;
}
