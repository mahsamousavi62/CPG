using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate.Specifications;

public class CompanyIPGIncludeProvider : Specification<CompanyIPG>, ISingleResultSpecification<CompanyIPG>
{
    public CompanyIPGIncludeProvider(long id)
    {
        Query.Where(c => c.IsActive & c.Id == id)
         .Include(c => c.Provider);
    }
}