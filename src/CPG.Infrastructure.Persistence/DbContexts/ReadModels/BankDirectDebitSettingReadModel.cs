using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class BankDirectDebitSettingReadModel
{
    public long Id { get; set; }
    public long ProviderId { get; set; }
    public int BankId { get; set; }
    public string DDBankCode { get; set; }
    public decimal MaxWithdrawalAmountPerDay { get; set; }
    public ValidityDuration MaxMandateValidityDurationPerMonth { get; set; }
    public AuthenticationType AuthenticationType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public BankReadModel Bank { get; set; }
    public ProviderReadModel Provider { get; set; }
}
