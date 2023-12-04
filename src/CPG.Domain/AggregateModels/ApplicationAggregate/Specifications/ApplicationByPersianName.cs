using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationByPersianName : Specification<Application>, ISingleResultSpecification<Application>
{
    public ApplicationByPersianName(string persianName)
    {
        Query.Where(c => c.PersianName == persianName);
    }
}