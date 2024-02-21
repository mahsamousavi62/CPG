using Ardalis.Specification;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;

public class CompanyDepositByIban : Specification<CompanyDeposit>, ISingleResultSpecification<CompanyDeposit>
{
    public CompanyDepositByIban(string iban)
    {
        Query.Include(c => c.Bank)
            .Include(c => c.Company)
            .Where(c => c.Iban == iban);
    }
}
