using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.BankAggregate.Specifications;

public sealed class ActiveBanksSpec : Specification<Bank>
{
    public ActiveBanksSpec()
    {
        Query.Include(bank => bank.DirectDebitSetting)
            .Where(bank =>  bank.IsActive == true)
            .OrderByDescending(bank => bank.Id);
    }
}
