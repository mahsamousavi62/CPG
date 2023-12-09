using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationIdentifierContainsIdpClientId : Specification<ApplicationIdentifier>, ISingleResultSpecification<ApplicationIdentifier>
{
    public ApplicationIdentifierContainsIdpClientId(string[] idpClientIds)
    {
        Query
            .Where(app => idpClientIds.Contains(app.IdpClientId));
    }
}