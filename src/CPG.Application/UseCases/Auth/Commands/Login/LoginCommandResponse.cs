namespace CPG.Application.UseCases.Auth.Commands.Login;

public class LoginCommandResponse(string token)
{
    public string Token { get; } = token;
}
