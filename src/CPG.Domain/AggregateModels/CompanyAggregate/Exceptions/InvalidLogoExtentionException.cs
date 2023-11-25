using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    internal class InvalidLogoExtentionException : DomainException
    {
        public override string Code => "invalid_logo_extention";
        public InvalidLogoExtentionException(string message) : base(message)
        {
        }
    }
}
