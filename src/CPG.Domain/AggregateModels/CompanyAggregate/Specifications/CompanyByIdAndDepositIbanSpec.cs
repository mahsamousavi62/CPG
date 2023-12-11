using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications;

public class CompanyByIdAndDepositIbanSpec : Specification<Company>, ISingleResultSpecification<Company>
{
    public CompanyByIdAndDepositIbanSpec(long companyId, string destinationIban)
    {
        Query.Include(a => a.CompanyDeposits.Where(b => b.IsActive && b.Bank.IsActive && b.Iban == destinationIban))
             .ThenInclude(c => c.Bank)
             .Include(a => a.PaymentMethods.Where(b => b.IsActive))             
             //.Include(a=> a.CompanyIPGs.Where(b => b.IsActive && b.IPGType.IsActive))
             //.ThenInclude(c => c.IPGType)
             //.Include(a => a.CompanyIPGs.Where(b => b.IsActive))
             //.ThenInclude(c => c.CompanyIPGDeposits.Where(d => d.IsActive))
             .Where(company => company.Id == companyId);
    }
}
