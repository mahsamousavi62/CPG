using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications;

public class CompanyPaymentMethodsByIdSpec : Specification<Company>, ISingleResultSpecification<Company>
{
    public CompanyPaymentMethodsByIdSpec(long companyId)
    {
        Query.Include(a => a.PaymentMethods.Where(b => b.IsActive))
             .Include(a => a.CompanyDeposits.Where(b => b.IsActive && b.Bank.IsActive))         
             .ThenInclude(c => c.Bank)
             .Include(a => a.CompanyIPGs.Where(b => b.IsActive && b.IPGType.IsActive && b.Provider.IsActive))
             .ThenInclude(c => c.IPGType)
             .Include(a => a.CompanyIPGs)
             .ThenInclude(c => c.Provider)
             .Include(a => a.CompanyIPGs)
             .ThenInclude(c => c.IPGDeposits.Where(d => d.IsActive))             
             .Where(company => company.Id == companyId);
    }
}