using CPG.Application.UseCases.Applications.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Applications.Commands.UpdateApplication;

public class UpdateApplicationCommand(UpdateApplicationViewModel model) : IRequest<Result<Unit>>
{
    public UpdateApplicationViewModel Model { get; } = model;
}
