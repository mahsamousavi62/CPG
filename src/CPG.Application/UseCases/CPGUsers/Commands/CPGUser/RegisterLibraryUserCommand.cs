using MediatR;

namespace CPG.Application.UseCases.CPGUsers.Commands.RegisterCPGUser
{
    public record RegisterCPGUserCommand(string Login, string Password, string FirstName, string LastName, string Email) : IRequest;
}