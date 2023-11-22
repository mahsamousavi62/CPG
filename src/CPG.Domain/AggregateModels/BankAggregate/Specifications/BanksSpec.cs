using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.BankAggregate.Specifications
{
    public sealed class BanksSpec : Specification<Bank>
    {
        public BanksSpec()
        {
            Query
                .Where(bank => bank.IsActive)
                .OrderByDescending(bank => bank.Id);
        }
    }
}
