namespace CPG.Application.UseCases.Exceptions;

public class UserAuthenticationException(string message) : ApplicationException(message)
{
    public override string Code => "user_authentication_error";
}
