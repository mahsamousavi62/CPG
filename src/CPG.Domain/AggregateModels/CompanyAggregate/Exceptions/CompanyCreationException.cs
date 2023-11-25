using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class CompanyCreationException : DomainException
    {
        public override string Code => "cannot_create_company";

        public CompanyCreationException(string message) : base(message)
        {
        }
    }
}
