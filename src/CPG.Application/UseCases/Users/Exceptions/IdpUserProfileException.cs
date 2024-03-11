using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.Users.Exceptions;

public class IdpUserProfileException() : AppException(string.Empty)
{
    public override string Code => "IdpUserProfile_Exception";
   
}
