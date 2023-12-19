using CCPG.Domain.SharedKernel.Communication.Ipg;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Communication.Ipg
{
    public interface IIpgFactory
    {
        IIpgProvider GetInstance(ProviderType providerType);
    }
}
