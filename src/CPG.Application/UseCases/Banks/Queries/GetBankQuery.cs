using CPG.Application.UseCases.Banks.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Banks.Queries
{
    public class GetBankQuery : IRequest<BankViewModel>
    {
        public int BankId { get; }

        public GetBankQuery(int bankId) => BankId = bankId;
    }
}
