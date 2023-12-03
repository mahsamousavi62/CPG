using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    internal class InvalidLogoExtentionException(string message)
        : DomainException(string.Format(Resource.invalid_logo_extention, message))
    {
        public override string Code => "invalid_logo_extention";
      
    }
}
