using CPG.Application.Shared.Resource;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Users.Exceptions;

public class UserNotFoundException(string idpId) : ApplicationException(string.Format(GlobalResource.UserNotFound, idpId))
{
    public override string Code => "user_not_found";
    public string IdpId { get; } = idpId;
}
