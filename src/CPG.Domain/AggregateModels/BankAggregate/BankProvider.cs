using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SeedWork;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.BankAggregate;

public class BankProvider : AuditableEntity<long>
{
    internal int _bankId;
    internal ProviderType _providerType;
    
    public int BankId => _bankId;
    public ProviderType ProviderType => _providerType;

    public Bank Bank { get; set; }
}
