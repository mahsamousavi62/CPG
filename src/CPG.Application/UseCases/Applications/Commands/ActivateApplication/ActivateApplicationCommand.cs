using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Application.Commands.ActivateApplication;

public record ActivateApplicationCommand(int ApplicationId, bool IsActive) : IRequest<Result<bool>>;
