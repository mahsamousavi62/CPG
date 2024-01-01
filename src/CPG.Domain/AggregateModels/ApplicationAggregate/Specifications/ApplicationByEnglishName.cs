using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationByEnglishName : Specification<Application>, ISingleResultSpecification<Application>
{
    public ApplicationByEnglishName(string englishName)
    {
        Query.Where(c => c.EnglishName == englishName);
    }
}