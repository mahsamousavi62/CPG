using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;

public class DirectDebitGrantByIdSpec : Specification<DirectDebitGrant>
{
    public DirectDebitGrantByIdSpec(long id)
    {
        Query.Where(t => t.Id == id);
    }
}