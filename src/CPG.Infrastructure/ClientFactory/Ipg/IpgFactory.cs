
using CCPG.Domain.SharedKernel.ClientFactory.Ipg;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ClientFactory.Ipg;

namespace CPG.Infrastructure.ClientFactory.Ipg
{
    public class IpgFactory : IIpgFactory
    {
        public IIpgService GetInstance(Enums.ProviderType providerType)
        {
           switch (providerType) 
            {
                case Enums.ProviderType.AsanPardakht:
                    {
                        return new AsanPardakhtService();
                    }
                    default: return null;
            }
        }
    }
}
