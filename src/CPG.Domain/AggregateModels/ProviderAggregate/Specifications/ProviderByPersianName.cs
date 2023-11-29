using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Specifications;

public class ProviderByPersianName : Specification<Provider>, ISingleResultSpecification<Provider>
{
    public ProviderByPersianName(string persianName)
    {
        Query.Where(c => c.PersianName == persianName);
    }
}
