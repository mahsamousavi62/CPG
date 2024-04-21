using CPG.Domain.SharedKernel;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class CompanyDepositPaymentMethodReadModel
{
    public long Id { get; set; }
    public Enums.PaymentMethodType MethodType { get; set; }
    public long CompanyDepositId { get; set; }
    public CompanyDepositReadModel CompanyDeposit { get; set; }
}
