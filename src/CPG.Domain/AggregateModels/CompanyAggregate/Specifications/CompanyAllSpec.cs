using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications
{
    public sealed class CompanyAllSpec : Specification<Company>, ISingleResultSpecification
    {
        public CompanyAllSpec()
        {

        }
    }
}