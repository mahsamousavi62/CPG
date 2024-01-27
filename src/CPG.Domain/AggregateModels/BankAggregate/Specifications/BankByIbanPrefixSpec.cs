using Ardalis.Specification;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
