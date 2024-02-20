using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationbyIdpClientIdUpdateMode : Specification<Application>, ISingleResultSpecification<Application>
{
    public ApplicationbyIdpClientIdUpdateMode(string[] idpClientIds,int id)
    {
        Query.Include(a => a.ApplicationIdentifiers)
             .Where(app => app.ApplicationIdentifiers.Any(a => idpClientIds.Contains(a.IdpClientId)) && app.Id!=id);
    }
}