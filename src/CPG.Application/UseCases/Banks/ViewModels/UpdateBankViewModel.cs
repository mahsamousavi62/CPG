using System.ComponentModel.DataAnnotations;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Banks.ViewModels;

public class UpdateBankViewModel
{
    [Required]
    public int BankId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string LogoAddress { get; set; }
    [Required]
    public string IbanPrefix { get; set; }
    [Required]
    public bool HasDirectDebitFeature { get; set; }
    public long ProviderId { get; set; }
    public string DDBankCode { get; set; }
    public decimal MaxWithdrawalAmountPerDay { get; set; }
    public ValidityDuration MaxMandateValidityDurationPerMonth { get; set; }
    public AuthenticationType AuthenticationType { get; set; }
}
