using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Specifications;

public class ProviderByPersianNameUpdateMode : Specification<Provider>, ISingleResultSpecification<Provider>
{
    public ProviderByPersianNameUpdateMode(string persianName,long id)
    {
        Query.Where(c => c.PersianName == persianName && c.Id!=id);
    }
}
