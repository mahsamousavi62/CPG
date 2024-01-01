using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions
{
    public class AllCompanyDepositsIsInActiveException : DomainException
    {
        public override string Code => "All_CompanyDeposits_InActive";
        public long CompanyId { get; }

        public AllCompanyDepositsIsInActiveException(long companyId) : base(string.Format(Resource.AllCompanyDepositsInActive, companyId))
           => CompanyId = companyId;
    }
}
