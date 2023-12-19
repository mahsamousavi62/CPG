using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.ClientFactory.Ipg
{
    public interface IIpgFactory
    {
        IIpgService GetInstance(ProviderType providerType);
    }
}
