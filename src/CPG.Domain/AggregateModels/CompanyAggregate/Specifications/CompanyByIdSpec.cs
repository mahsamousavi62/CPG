using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications
{
    public sealed class CompanyByIdSpec : Specification<Company>, ISingleResultSpecification
    {
        public CompanyByIdSpec(long companyId)
        {
            Query
                .Where(company => company.Id == companyId);
        }
    }
}