using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate.Specifications;

public class CompanyIPGById : Specification<CompanyIPG>, ISingleResultSpecification<CompanyIPG>
{
    public CompanyIPGById(long id)
    {
        Query.Where(c => c.Id == id)
         .Include(c => c.IPGDeposits);
    }
}