using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationByIdSpec : Specification<Application>, ISingleResultSpecification<Application>
{
    public ApplicationByIdSpec(int appId)
    {
        Query
            .Where(app => app.Id == appId);
    }
}