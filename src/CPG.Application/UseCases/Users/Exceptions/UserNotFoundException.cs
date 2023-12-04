using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Users.Exceptions;

public class UserNotFoundException(long userId) : ApplicationException($"User with ID {userId} has not been found.")
{
    public override string Code => "user_not_found";
    public long UserId { get; } = userId;
}
