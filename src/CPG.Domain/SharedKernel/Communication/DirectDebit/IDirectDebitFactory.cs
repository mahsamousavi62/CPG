using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit;

public interface IDirectDebitFactory
{
    IDirectDebitProvider GetInstance(ProviderType providerType);
}
