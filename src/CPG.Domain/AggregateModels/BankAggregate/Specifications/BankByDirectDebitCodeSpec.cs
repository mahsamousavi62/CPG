using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.BankAggregate.Specifications;

public class BankByDirectDebitCodeSpec : Specification<Bank>, ISingleResultSpecification<Bank>
{
    public BankByDirectDebitCodeSpec(string bankCode)
    {
        Query.Include(bank => bank.DirectDebitSetting)
            .Where(bank => bank.DirectDebitSetting.DDBankCode == bankCode);
    }
}