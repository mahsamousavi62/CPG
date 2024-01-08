using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;


namespace CPG.Application.UseCases.Companies.Exceptions
{
    public class UsersNotFoundException() : AppException(string.Format(GlobalResource.UsersNotFound))
    {
        public override string Code => "users_not_found";
    }
}
