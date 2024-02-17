using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.CompanyIPGs.ViewModels;

public class UpdateCompanyIPGModel
{
    [Required]
    public long Id { get; set; }
    
    [Required]
    public string ProviderData { get; set; }

    [Required]
    public CreateCompanyIPGDepositModel[] CompanyIPGDeposits { get; set; }
}

