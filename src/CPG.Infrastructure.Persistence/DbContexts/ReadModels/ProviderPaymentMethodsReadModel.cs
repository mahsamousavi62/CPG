using CPG.Domain.SharedKernel;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class ProviderPaymentMethodReadModel
{
    public long Id { get; set; }
    public Enums.PaymentMethodType MethodType { get; set; }
    public long ProviderId { get; set; }
    public ProviderReadModel Provider { get; set; }
}

