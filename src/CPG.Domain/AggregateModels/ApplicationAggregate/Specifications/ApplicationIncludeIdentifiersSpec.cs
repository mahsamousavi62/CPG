using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationIncludeIdentifiersSpec : Specification<Application>, ISingleResultSpecification<Application>
{
    public ApplicationIncludeIdentifiersSpec()
    {
        Query.Include(a=>a.ApplicationIdentifiers);
    }
}