using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;

public class CreateCompanyIPGViewModel(long companyId,
    long providerId,
    long iPGTypeId,
    string providerData,
    List<CreateCompanyIPGDepositViewModel> companyIPGDeposits
    )
{
    public long CompanyId { get; set; } = companyId;
    public long ProviderId { get; set; } = providerId;
    public long IPGTypeId { get; set; } = iPGTypeId;
    public string ProviderData { get; set; } = providerData;
    public List<CreateCompanyIPGDepositViewModel> CompanyIPGDeposits { get; set; } = companyIPGDeposits;
}
