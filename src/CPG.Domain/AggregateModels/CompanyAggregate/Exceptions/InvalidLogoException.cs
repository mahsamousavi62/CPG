using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class InvalidLogoException:DomainException
    {
        public override string Code => "invalid_logo";
        public InvalidLogoException(string message) : base(message)
        {
        }
    }
}
