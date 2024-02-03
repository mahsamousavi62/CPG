using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.ProviderAggregate.Specifications;

public class ProviderById : Specification<Provider>, ISingleResultSpecification<Provider>
{
    public ProviderById(long id)
    {
        Query.Include(p=>p.PaymentMethods).Where(c => c.Id==id);
    }
}
