using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;

public class DirectDebitGrantByAuthorizationIdSpec : Specification<DirectDebitGrant>
{
    public DirectDebitGrantByAuthorizationIdSpec(string authorizationId)
    {
        Query.Where(t => t.AuthorizationId == authorizationId);
    }
}