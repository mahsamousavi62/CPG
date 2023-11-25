using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.BankAggregate.Specifications;

public sealed class ActiveBanksSpec : Specification<Bank>
{
    public ActiveBanksSpec()
    {
        Query
            .Where(bank =>  bank.Status == SharedKernel.Enums.BankStatus.Active)
            .OrderByDescending(bank => bank.Id);
    }
}
