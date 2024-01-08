using CPG.Application.Shared.Resource;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.Users.Exceptions;

public class UserNotFoundException(string idpId) : AppException(string.Format(GlobalResource.UserNotFound, idpId))
{
    public override string Code => "user_not_found";
    public string IdpId { get; } = idpId;
}
