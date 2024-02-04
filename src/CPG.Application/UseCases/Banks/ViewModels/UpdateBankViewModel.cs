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
    public string Logo { get; set; }
    [Required]
    public string IbanPrefix { get; set; }
    [Required]
    public bool HasDirectDebitFeature { get; set; }
    public UpdateBankDirectDebitSettingViewModel DirectDebitSetting { get; set; }
}
