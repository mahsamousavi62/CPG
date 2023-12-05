using Ardalis.Specification;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.AggregateModels.CompanyAggregate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications
{
    public class CompanyDepositByIban : Specification<CompanyDeposit>, ISingleResultSpecification<CompanyDeposit>
    {
        public CompanyDepositByIban(string iban)
        {
            Query.Where(c => c.Iban == iban);
        }
    }
}
