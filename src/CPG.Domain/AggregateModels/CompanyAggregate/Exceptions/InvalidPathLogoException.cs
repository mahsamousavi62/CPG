using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class InvalidPathLogoException(string message) :
        DomainException(string.Format(Resource.Invalid_Logo_path, message))
    {
        public override string Code => "invalid_logo_path";
    }
}
