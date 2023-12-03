using MediatR;

namespace CPG.Application.UseCases.Application.Commands.CreateApplication;

public record CreateApplicationCommand(CreateApplicationViewModel Model) : IRequest<long>;
