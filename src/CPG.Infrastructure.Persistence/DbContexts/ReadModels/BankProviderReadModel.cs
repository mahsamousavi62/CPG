using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class BankProviderReadModel
{
    public long Id { get; set; }
    public int BankId { get; set; }
    public ProviderType ProviderType { get; set; }
    public bool IsActive { get; set; }
    public BankReadModel Bank { get; set; }
}
