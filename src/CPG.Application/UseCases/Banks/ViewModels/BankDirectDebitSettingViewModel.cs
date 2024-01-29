using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Banks.ViewModels;

public class BankDirectDebitSettingViewModel
{
    public long Id { get; set; }
    public long ProviderId { get; set; }
    public int BankId { get; set; }
    public string DDBankCode { get; set; }
    public decimal MaxWithdrawalAmountPerDay { get; set; }
    public ValidityDuration MaxMandateValidityDurationPerMonth { get; set; }
    public AuthenticationType AuthenticationType { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public BankViewModel Bank { get; set; }
}
