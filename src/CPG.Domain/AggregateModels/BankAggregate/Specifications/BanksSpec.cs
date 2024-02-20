using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.BankAggregate.Specifications;

public sealed class BanksSpec : Specification<Bank>
{
    public BanksSpec()
    {
        Query.OrderByDescending(bank => bank.Id);
    }
}