using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class CompanyCreationException(string message) : DomainException(string.Format(Resource.cannot_create_company))
    {
        public override string Code => "cannot_create_company";
    }
}
