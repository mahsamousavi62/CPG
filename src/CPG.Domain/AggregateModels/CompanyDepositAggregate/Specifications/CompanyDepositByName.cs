using Ardalis.Specification;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications
{
    public class CompanyDepositByName : Specification<CompanyDeposit>, ISingleResultSpecification<CompanyDeposit>
    {
        public CompanyDepositByName(string name)
        {
            Query.Where(c => c.Name == name);
        }
    }
}
