namespace CPG.Application.UseCases.Exceptions;

public class UserAlreadyExistsException(string email) : AppException($"CPG user already exists with email {email}.")
{
    public override string Code => "CPG_user_already_exists";
    public string Email { get; } = email;
}
