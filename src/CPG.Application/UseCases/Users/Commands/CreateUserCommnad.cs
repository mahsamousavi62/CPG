using MediatR;

namespace CPG.Application.UseCases.Users.Commands
{
    public record CreateUserCommnad(string IDPId) : IRequest;

}
