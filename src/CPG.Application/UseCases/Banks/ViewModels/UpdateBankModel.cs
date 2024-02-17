using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Banks.ViewModels;

public class UpdateBankModel
{
    [Required]
    public int BankId { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public IFormFile Logo { get; set; }
    [Required]
    public string IbanPrefix { get; set; }
    [Required]
    public bool HasDirectDebitFeature { get; set; }
    public UpdateBankDirectDebitSettingViewModel DirectDebitSetting { get; set; }
}
