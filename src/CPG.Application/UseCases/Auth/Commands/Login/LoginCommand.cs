using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Auth.Commands.Login;

public record LoginCommand(string Login, string Password) : IRequest<Result<LoginCommandResponse>>;
