using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationByEnglishNameUpdateMode : Specification<Application>, ISingleResultSpecification<Application>
{
    public ApplicationByEnglishNameUpdateMode(string englishName, int id)
    {
        Query.Where(c => c.EnglishName == englishName && c.Id != id);
    }
}

