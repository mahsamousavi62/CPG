using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate
{
    public class CompanyIPGDeposit : AuditableEntity<long>
    {
        public CompanyIPGDeposit()
        {
            
        }
        public long CompanyIPGId { get; set; }
        public CompanyIPG CompanyIPG { get; set; }
        public long CompanyDepositId { get; set; }
        public CompanyDeposit CompanyDeposit { get; set; }
        public bool IsDefault { get; set; }
    }
}
