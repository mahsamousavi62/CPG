using CPG.Application.UseCases.Providers.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Providers.Queries;

public class GetProviderQuery(long providerId) : IRequest<ProviderViewModel>
{
    public long ProviderId { get; } = providerId;
}