using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyLogoException:DomainException
    {
        public override string Code => "empty_logo";
        public EmptyLogoException(string message) : base(message)
        {
        }

    }
}
