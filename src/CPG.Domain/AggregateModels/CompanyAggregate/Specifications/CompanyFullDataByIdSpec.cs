using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications;

public class CompanyFullDataByIdSpec : Specification<Company>, ISingleResultSpecification<Company>
{
    public CompanyFullDataByIdSpec(long companyId)
    {
        Query.Include(a => a.PaymentMethods.Where(b => b.IsActive))
             .Include(a => a.CompanyDeposits.Where(b => b.IsActive && b.Bank.IsActive))         
             .ThenInclude(c => c.Bank)
             .Include(a=> a.CompanyIPGs.Where(b => b.IsActive && b.IPGType.IsActive))
             .ThenInclude(c => c.IPGType)
             .Include(a => a.CompanyIPGs.Where(b => b.IsActive))
             .ThenInclude(c => c.IPGDeposits.Where(d => d.IsActive))
             .Include(a => a.CompanyIPGs.Where(b => b.IsActive && b.Provider.IsActive))
             .ThenInclude(c => c.Provider)
             .Where(company => company.Id == companyId);
    }
}