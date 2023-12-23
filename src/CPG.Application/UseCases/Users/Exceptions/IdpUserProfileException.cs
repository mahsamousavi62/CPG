using System.Reflection.Metadata;
using CPG.Application.Shared.Resource;
using CPG.Domain.AggregateModels.UserAggregate;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Users.Exceptions
{
    public class IdpUserProfileException(string error) : ApplicationException(string.Format(GlobalResource.IdpUserProfileException, error))
    {
        public override string Code => "IdpUserProfile_Exception";
        public string Error { get; } = error;
    }
}
