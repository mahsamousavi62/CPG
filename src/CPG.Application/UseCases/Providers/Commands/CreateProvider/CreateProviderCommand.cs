using MediatR;

namespace CPG.Application.UseCases.Providers.Commands.CreateProvider;

public record CreateProviderCommand(CreateProviderViewModel Model) : IRequest<long>;
