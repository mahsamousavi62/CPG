using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Specifications;

public class ProviderByEnglishName : Specification<Provider>, ISingleResultSpecification<Provider>
{
    public ProviderByEnglishName(string englishName)
    {
        Query.Where(c => c.EnglishName == englishName);
    }
}
