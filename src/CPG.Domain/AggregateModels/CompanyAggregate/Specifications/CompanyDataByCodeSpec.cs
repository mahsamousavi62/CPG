using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications;

public class CompanyDataByCodeSpec : Specification<Company>, ISingleResultSpecification<Company>
{
    public CompanyDataByCodeSpec(short companyCode)
    {
        Query.Include(c => c.CompanyDeposits)
            .ThenInclude(c => c.PaymentMethods)
            .Include(c => c.CompanyDeposits)
            .ThenInclude(c => c.Bank)
            .Include(a => a.PaymentMethods)
            .Include(c => c.ShaparakSetting)
            .Include(c => c.Users).ThenInclude(c => c.UserRoles)
            .Where(company => company.Code == companyCode);
    }
}