using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.AggregateModels.CompanyIPGAggregate.Exceptions;
using CPG.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate
{
    public class CompanyIPGDeposit : AuditableEntity<long>
    {
        public CompanyIPGDeposit(long companyIpgId, long companyDepositId, bool isDefault)
        {
            CompanyIPGId = companyIpgId;
            CompanyDepositId = companyDepositId;
            IsDefault = isDefault;
        }

        public CompanyIPGDeposit(long companyDepositId, bool isDefault)
        {
            CompanyDepositId = companyDepositId;
            IsDefault = isDefault;
        }

        public long CompanyIPGId { get; set; }
        public CompanyIPG CompanyIPG { get; set; }
        public long CompanyDepositId { get; set; }
        public CompanyDeposit CompanyDeposit { get; set; }
        public bool IsDefault { get; set; }

        public static List<CompanyIPGDeposit> Create(CompanyIPGDeposit[] values)
        {
            if (values is null || values.Length == 0)
                throw new ArgumentNullException(nameof(values));

            if (values.Select(x => x.CompanyDepositId).Distinct().Count() != values.Length)
                throw new DuplicateDepositException(string.Empty);

            if (values.Count(x => x.IsDefault == true) > 1)
                throw new MultipleDefaultDepositException(string.Empty);

            return values.ToList();
        }
    }
}
