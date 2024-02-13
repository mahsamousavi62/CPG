using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.BankAggregate.Specifications
{
    public class BankByIbanPrefixSpec : Specification<Bank>, ISingleResultSpecification<Bank>
    {
        public BankByIbanPrefixSpec(string ibanPrefix)
        {
            Query.Include(bank => bank.DirectDebitSetting)
                .Where(bank => bank.IbanPrefix.Value == ibanPrefix);
        }
    }
}
