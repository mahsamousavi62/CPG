using MediatR;

namespace Daryaftyar.Application.UseCases.DaryaftyarUsers.Commands.RegisterDaryaftyarUser
{
    public record RegisterDaryaftyarUserCommand(string Login, string Password, string FirstName, string LastName, string Email) : IRequest;
}