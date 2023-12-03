using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class InvalidLogoException(string message) 
        : DomainException(string.Format(Resource.Invalid_logo, message))
    {
        public override string Code => "invalid_logo";
    }
}
