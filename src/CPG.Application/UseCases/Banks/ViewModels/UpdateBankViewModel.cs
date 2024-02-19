using System.ComponentModel.DataAnnotations;
using CPG.Domain.SharedKernel.File;
using Microsoft.AspNetCore.Http;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Banks.ViewModels;

public class UpdateBankViewModel
{
    
    public int BankId { get; set; }

    public UpdateBankViewModel(int bankId, string name, IFile logo, string ibanPrefix, bool hasDirectDebitFeature, UpdateBankDirectDebitSettingViewModel directDebitSetting )
    {
        BankId = bankId;
        Name = name;
        Logo = logo;
        IbanPrefix = ibanPrefix;
        HasDirectDebitFeature = hasDirectDebitFeature;
        DirectDebitSetting = directDebitSetting;
    }

    public string Name { get; set; }
   
    public IFile Logo { get; set; }
   
    public string IbanPrefix { get; set; }
    
    public bool HasDirectDebitFeature { get; set; }
    public UpdateBankDirectDebitSettingViewModel DirectDebitSetting { get; set; }
}
