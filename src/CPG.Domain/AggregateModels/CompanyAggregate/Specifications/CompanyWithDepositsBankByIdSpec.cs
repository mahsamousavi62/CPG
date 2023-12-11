using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications;

public class CompanyWithDepositsBankByIdSpec : Specification<Company>, ISingleResultSpecification<Company>
{
    public CompanyWithDepositsBankByIdSpec(long companyId)
    {
        Query.Include(a => a.PaymentMethods.Where(b => b.IsActive))
             .Include(a => a.CompanyDeposits.Where(b => b.IsActive && b.Bank.IsActive))         
             .ThenInclude(c => c.Bank)
             //.Include(a=> a.CompanyIPGs.Where(b => b.IsActive))
             //.ThenInclude(c => c.IPGType)
             //.Include(a => a.CompanyIPGs.Where(b => b.IsActive))
             //.ThenInclude(c => c.CompanyIPGDeposits.Where(d => d.IsActive))
             .Where(company => company.Id == companyId);
    }
}