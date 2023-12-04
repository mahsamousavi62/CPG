using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationsSpec : Specification<Application>
{
    public ApplicationsSpec()
    {
        Query
            .OrderByDescending(app => app.IsActive);
    }
}