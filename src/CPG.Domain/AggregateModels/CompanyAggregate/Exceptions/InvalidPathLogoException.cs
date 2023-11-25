using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class InvalidPathLogoException:DomainException
    {
        public override string Code => "invalid_path_logo";
        public InvalidPathLogoException(string message) : base(message)
        {
        }

    }
}
