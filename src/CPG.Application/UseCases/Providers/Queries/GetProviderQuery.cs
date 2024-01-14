using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Providers.Queries;

public class GetProviderQuery(long providerId) : IRequest<Result<ProviderViewModel>>
{
    public long ProviderId { get; } = providerId;
}