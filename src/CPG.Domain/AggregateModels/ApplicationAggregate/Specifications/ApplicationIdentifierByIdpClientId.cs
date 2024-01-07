using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationbyIdpClientId : Specification<Application>, ISingleResultSpecification<Application>
{
    public ApplicationbyIdpClientId(string[] idpClientIds)
    {
        Query.Include(a => a.ApplicationIdentifiers)
             .Where(app => app.ApplicationIdentifiers.Any(a => idpClientIds.Contains(a.IdpClientId)));
    }
}