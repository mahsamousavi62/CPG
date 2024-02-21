using Ardalis.Specification;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;

public class DefaultDirectDebitDepositSpec : Specification<CompanyDeposit>, ISingleResultSpecification<CompanyDeposit>
{
    public DefaultDirectDebitDepositSpec(long companyId)
    {
        Query.Include(c => c.Bank)
            .Where(c => c.CompanyId == companyId && c.IsDefaultForDirectDebit == true);
    }
}