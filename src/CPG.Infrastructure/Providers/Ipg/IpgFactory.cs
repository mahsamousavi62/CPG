
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Ipg;

namespace CPG.Infrastructure.Providers.Ipg
{
    public class IpgFactory : IIpgFactory
    {
        public IIpgProvider GetInstance(Enums.ProviderType providerType)
        {
           switch (providerType) 
            {
                case Enums.ProviderType.AsanPardakht:
                    {
                        return new AsanPardakhtProvider();
                    }
                    default: return null;
            }
        }
    }
}
