using Ardalis.Specification;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;

public class DefaultCharismaCardDepositSpec : Specification<CompanyDeposit>,
    ISingleResultSpecification<CompanyDeposit>
{
    public DefaultCharismaCardDepositSpec(long companyId)
    {
        Query.Include(c => c.Bank)
            .Where(c => c.CompanyId == companyId && c.IsDefaultForCharismaCard==true);
    }
}