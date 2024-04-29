using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications;

public class CompanyByCodeSpec : Specification<Company>, ISingleResultSpecification<Company>
{
    public CompanyByCodeSpec(short code, long? id = null)
    {
        if (id.HasValue)
        {
            Query.Where(c => c.Code == code && c.Id != id);
        }
        else
        {
            Query.Where(c => c.Code == code);
        }
    }
}