using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.Users.Exceptions;

public class IdpUserProfileException(string error) : AppException(error)
{
    public override string Code => "IdpUserProfile_Exception";
    public string Error { get; } = error;
}
