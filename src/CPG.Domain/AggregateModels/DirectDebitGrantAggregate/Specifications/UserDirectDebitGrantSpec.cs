using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;

public class UserDirectDebitGrantSpec : Specification<DirectDebitGrant>
{
    public UserDirectDebitGrantSpec(long userId)
    {
        Query.Where(t => t.UserId == userId);
    }
}