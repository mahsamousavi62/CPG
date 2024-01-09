using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Banks.Queries
{
    public class GetActiveBanksQuery : IRequest<Result<IReadOnlyCollection<BankViewModel>>>
    {
    }
}
