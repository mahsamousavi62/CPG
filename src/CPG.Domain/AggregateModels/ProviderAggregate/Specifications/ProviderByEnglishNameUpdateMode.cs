using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Specifications;

public class ProviderByEnglishNameUpdateMode  : Specification<Provider>, ISingleResultSpecification<Provider>
{
    public ProviderByEnglishNameUpdateMode(string englishName,long id)
    {
        Query.Where(c => c.EnglishName == englishName && c.Id!=id);
    }
}
