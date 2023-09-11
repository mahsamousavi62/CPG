using MediatR;

namespace Daryaftyar.Application.UseCases.Auth.Commands.Login
{
    public record LoginCommand(string Login, string Password) : IRequest<LoginCommandResponse>;
}
