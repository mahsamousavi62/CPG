using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ActiveApplicationsSpec : Specification<Application>
{
    public ActiveApplicationsSpec()
    {
        Query
            .Where(app => app.IsActive == true)
            .OrderByDescending(app => app.Id);
    }
}