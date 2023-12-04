using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SeedWork;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate
{
    public class CompanyIPG : AuditableEntity<long>, IAggregateRoot
    {
        public CompanyIPG()
        {

        }

        public CompanyIPG(long companyId, long providerId, int bankProviderId,
            short verificationTimeLimit, string providerData)
        {
            CompanyId = companyId;
            ProviderId = providerId;
            BankProviderId = bankProviderId;
            VerificationTimeLimit = verificationTimeLimit;
            ProviderData = providerData;
        }

        public long CompanyId { get; set; }
        public long ProviderId { get; set; }
        public int BankProviderId { get; set; }
        public string ProviderData { get; set; }
        public short VerificationTimeLimit { get; set; }
        public Company Company { get; set; }
        public Provider Provider { get; set; }



        public static CompanyIPG Create(long companyId, long providerId, int bankProviderId,
            short verificationTimeLimit, string providerData)
        {
            return new CompanyIPG(companyId, providerId, bankProviderId,
                                  verificationTimeLimit, providerData);
        }

    }
}
