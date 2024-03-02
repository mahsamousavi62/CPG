using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;

public class AllDirectDebitGrantByUserSpec : Specification<DirectDebitGrant>
{
    public AllDirectDebitGrantByUserSpec(long userId)
    {
        Query.Where(t => t.UserId == userId);
    }
}