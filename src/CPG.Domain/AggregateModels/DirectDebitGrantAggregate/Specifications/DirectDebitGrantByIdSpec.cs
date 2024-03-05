using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;

public class DirectDebitGrantByIdSpec : Specification<DirectDebitGrant>
{
    public DirectDebitGrantByIdSpec(long id)
    {
        Query.Include(t => t.Bank)
            .ThenInclude(t => t.DirectDebitSetting)
            .ThenInclude(t => t.Provider)
            .ThenInclude(t => t.PaymentMethods)
            .Where(t => t.Id == id);
    }
}