using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SeedWork;
using System.Collections.Generic;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate
{
    public class CompanyIPG : AuditableEntity<long>, IAggregateRoot
    {
        public CompanyIPG()
        {
        }

        public CompanyIPG(long companyId, long providerId, long ipgTypeId, string providerData)
        {
            CompanyId = companyId;
            ProviderId = providerId;
            IPGTypeId = ipgTypeId;            
            ProviderData = providerData;
            IsActive = true;
        }

        public long CompanyId { get; set; }
        public long ProviderId { get; set; }
        public long IPGTypeId { get; set; }
        public string ProviderData { get; set; }
        public Company Company { get; set; }
        public Provider Provider { get; set; }
        public IPGType IPGType { get; set; }
        public List<CompanyIPGDeposit> IPGDeposits { get; set; }

        public static CompanyIPG Create(long companyId, long providerId, long ipgTypeId, string providerData, CompanyIPGDeposit[] details)
        {
            var companyIpg = new CompanyIPG(companyId, providerId, ipgTypeId, providerData);
            var ipgDeposits = CompanyIPGDeposit.Create(details);
            companyIpg.IPGDeposits = ipgDeposits;
            return companyIpg;
        }
    }
}
