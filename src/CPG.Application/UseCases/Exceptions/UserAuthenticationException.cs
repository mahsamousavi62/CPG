namespace CPG.Application.UseCases.Exceptions;

public class UserAuthenticationException(string message) : AppException(message)
{
    public override string Code => "user_authentication_error";
}
