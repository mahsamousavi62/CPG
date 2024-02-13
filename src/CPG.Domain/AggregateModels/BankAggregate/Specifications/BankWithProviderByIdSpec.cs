using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.BankAggregate.Specifications;

public class BankWithProviderByIdSpec : Specification<Bank>, ISingleResultSpecification<Bank>
{
    public BankWithProviderByIdSpec(int bankId)
    {
        Query.Include(q => q.DirectDebitSetting)
            .ThenInclude(q => q.Provider)
            .ThenInclude(q => q.PaymentMethods)
            .Where(q => q.Id == bankId);
    }
}