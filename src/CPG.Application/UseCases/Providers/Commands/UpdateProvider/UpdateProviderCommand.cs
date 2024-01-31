using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Providers.Commands.UpdateProvider;

public class UpdateProviderCommand(UpdateProviderViewModel model) : IRequest<Result<Unit>>
{
    public UpdateProviderViewModel Model { get; } = model;
}
