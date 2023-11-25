using CPG.Application.UseCases.Banks.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Banks.Queries
{
    public class GetActiveBanksQuery : IRequest<IReadOnlyCollection<BankViewModel>>
    {
    }
}
