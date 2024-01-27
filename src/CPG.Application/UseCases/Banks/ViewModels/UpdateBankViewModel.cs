using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Banks.ViewModels;

public class UpdateBankViewModel
{
    public int BankId { get; set; }
    public string Name { get; set; }
    public string LogoAddress { get; set; }
    public string IbanPrefix { get; set; }
    public bool HasDirectDebitFeature { get; set; }
    public long ProviderId { get; set; }
    public string DDBankCode { get; set; }
    public decimal MaxWithdrawalAmountPerDay { get; set; }
    public ValidityDuration MaxMandateValidityDurationPerMonth { get; set; }
    public AuthenticationType AuthenticationType { get; set; }
}
