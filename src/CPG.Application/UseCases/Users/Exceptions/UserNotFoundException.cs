using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Users.Exceptions
{
    public class UserNotFoundException : ApplicationException
    {
        public override string Code => "user_not_found";
        public long UserId { get; }

        public UserNotFoundException(long userId) : base($"User with ID {userId} has not been found.")
            => UserId = userId;
    }
}
