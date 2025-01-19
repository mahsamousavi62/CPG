namespace CPG.Application.UseCases.Users.Commands;

public record CreateUserCommand : IRequest<Result<long>>;
