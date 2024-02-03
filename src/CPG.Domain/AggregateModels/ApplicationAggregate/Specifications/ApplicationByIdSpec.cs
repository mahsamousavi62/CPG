using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationByIdSpec : Specification<Application>, ISingleResultSpecification<Application>
{
    public ApplicationByIdSpec(int appId)
    {
        Query.Include(a=>a.ApplicationCallbackUrls)
            .Include(a=>a.ApplicationIdentifiers)
            .Where(app => app.Id == appId);
    }
}