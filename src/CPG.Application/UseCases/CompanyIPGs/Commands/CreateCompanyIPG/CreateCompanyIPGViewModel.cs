using CPG.Application.UseCases.CompanyIPGs.ViewModels;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;

public class CreateCompanyIPGViewModel(long companyId,
    long providerId,
    long iPGTypeId,
    string providerData,
    short verificationTimeLimit,
    CompanyIPGDepositViewModel[] companyIPGDeposits
    )
{
    public long CompanyId { get; set; } = companyId;
    public long ProviderId { get; set; } = providerId;
    public long IPGTypeId { get; set; } = iPGTypeId;
    public string ProviderData { get; set; } = providerData;
    public short VerificationTimeLimit { get; set; } = verificationTimeLimit;
    public CompanyIPGDepositViewModel[] CompanyIPGDeposits { get; set; } = companyIPGDeposits;
}
