using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SeedWork;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate
{
    public class CompanyIPG : AuditableEntity<long>, IAggregateRoot
    {
        public CompanyIPG()
        {
        }

        public CompanyIPG(long companyId, long providerId, long ipgTypeId, short verificationTimeLimit, string providerData)
        {
            CompanyId = companyId;
            ProviderId = providerId;
            IPGTypeId = ipgTypeId;
            VerificationTimeLimit = verificationTimeLimit;
            ProviderData = providerData;
        }

        public long CompanyId { get; set; }
        public long ProviderId { get; set; }
        public long IPGTypeId { get; set; }
        public string ProviderData { get; set; }
        public short VerificationTimeLimit { get; set; }
        public Company Company { get; set; }
        public Provider Provider { get; set; }
        public IPGType IPGType { get; set; }
        public ICollection<CompanyIPGDeposit> IPGDeposits { get; set; }

        public static CompanyIPG Create(long companyId, long providerId, long ipgTypeId, short verificationTimeLimit, string providerData, CompanyIPGDeposit[] details)
        {
            var companyIpg = new CompanyIPG(companyId, providerId, ipgTypeId, verificationTimeLimit, providerData);
            var ipgDeposits = CompanyIPGDeposit.Create(details.Select(t => new { t.CompanyDepositId, t.IsDefault }).ToArray());
            //companyIpg.IPGDeposits.AddRange(ipgDeposits);
            return companyIpg;
        }
    }
}
