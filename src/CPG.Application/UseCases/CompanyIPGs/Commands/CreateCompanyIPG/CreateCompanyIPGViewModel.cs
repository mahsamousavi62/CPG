using CPG.Application.UseCases.CompanyIPGs.ViewModels;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;

public class CreateCompanyIPGViewModel
{
    public long CompanyId { get; set; }
    public long ProviderId { get; set; }
    public long IPGTypeId { get; set; }
    public string ProviderData { get; set; }
    public short VerificationTimeLimit { get; set; }
    public CompanyIPGDepositViewModel[] CompanyIPGDeposits { get; set; }
}
