using CPG.Application.UseCases.Providers.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Providers.Queries;

public class GetProviderQuery(int providerId) : IRequest<ProviderViewModel>
{
    public int ProviderId { get; } = providerId;
}