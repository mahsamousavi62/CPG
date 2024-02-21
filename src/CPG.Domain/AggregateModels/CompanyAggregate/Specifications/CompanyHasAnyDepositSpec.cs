using Ardalis.Specification;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications;

public class CompanyHasAnyDepositSpec : Specification<CompanyDeposit>, ISingleResultSpecification<CompanyDeposit>
{
    public CompanyHasAnyDepositSpec(long companyId)
    {
        Query.Where(c => c.CompanyId == companyId).Take(1);
    }
}
