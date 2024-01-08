using System.Reflection.Metadata;
using CPG.Application.Shared.Resource;
using CPG.Domain.AggregateModels.UserAggregate;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.Users.Exceptions
{
    public class IdpUserProfileException(string error) : AppException(string.Format(GlobalResource.IdpUserProfileException, error))
    {
        public override string Code => "IdpUserProfile_Exception";
        public string Error { get; } = error;
    }
}
