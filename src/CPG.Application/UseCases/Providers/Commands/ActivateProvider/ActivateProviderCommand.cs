using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Providers.Commands.ActivateProvider;

public record ActivateProviderCommand(int ProviderId, bool IsActive) : IRequest<Result<bool>>;