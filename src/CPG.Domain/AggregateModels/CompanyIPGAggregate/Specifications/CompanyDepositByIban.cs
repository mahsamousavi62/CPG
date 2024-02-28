using System.Linq;
using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate.Specifications
{
    public class CompanyIPGByIpgDeposit : Specification<CompanyIPG>, ISingleResultSpecification<CompanyIPG>
    {
        public CompanyIPGByIpgDeposit(long id)
        {
            Query.Where(c => c.IsActive & c.Id == id)
             .Include(c => c.IPGDeposits.Where(d => d.IsActive && d.IsDefault))
             .ThenInclude(d => d.CompanyDeposit)
             .ThenInclude(d => d.Bank);
        }
    }
}
