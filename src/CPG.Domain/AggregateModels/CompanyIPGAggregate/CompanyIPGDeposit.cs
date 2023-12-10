using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
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

        public static List<CompanyIPGDeposit> Create(dynamic[] values)
        {
            if (values is null || values.Length == 0)
                throw new ArgumentNullException(nameof(values));

            if (values.Select(x => x.Item1).Distinct().Count() != values.Length)
                throw new DuplicatePaymentMethodTypeException(nameof(values));

            if (values.Count(x => x.Item2) > 0)
                throw new InvalidPaymentMethodType(nameof(values));

            var ipgDeposits = values.Select(i => new CompanyIPGDeposit(i.Item1, i.Item2)).ToList();
            return ipgDeposits;
        }
    }
}
