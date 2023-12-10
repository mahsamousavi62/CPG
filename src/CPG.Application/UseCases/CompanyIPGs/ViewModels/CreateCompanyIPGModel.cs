using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.CompanyIPGs.ViewModels;

public class CreateCompanyIPGModel
{
    [Required]
    public long CompanyId { get; set; }

    [Required]
    public long ProviderId { get; set; }

    [Required]
    public long IPGTypeId { get; set; }

    [Required]
    public string ProviderData { get; set; }

    [Required]
    public short VerificationTimeLimit { get; set; }

    [Required]
    public CompanyIPGDepositViewModel[] CompanyIPGDeposits { get; set; }
}
