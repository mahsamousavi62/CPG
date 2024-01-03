using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications;

public sealed class CompanyByIdSpec : Specification<Company>, ISingleResultSpecification<Company>
{
    public CompanyByIdSpec(long companyId)
    {
        Query.Include(c => c.CompanyDeposits)
            .Where(company => company.Id == companyId);
    }
}