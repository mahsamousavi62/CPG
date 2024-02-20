using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.BankAggregate.Specifications;

public sealed class BankByIdSpec : Specification<Bank>, ISingleResultSpecification<Bank>
{
    public BankByIdSpec(int bankId)
    {
        Query.Include(t => t.DirectDebitSetting)
            .ThenInclude(t => t.Provider)
            .Where(t => t.Id == bankId);
    }
}
