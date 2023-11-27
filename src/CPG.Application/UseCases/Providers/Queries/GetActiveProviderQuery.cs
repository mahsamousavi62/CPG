using CPG.Application.UseCases.Providers.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Providers.Queries
{
    public class GetActiveProviderQuery : IRequest<IReadOnlyCollection<ProviderViewModel>>
    {
    }
}
