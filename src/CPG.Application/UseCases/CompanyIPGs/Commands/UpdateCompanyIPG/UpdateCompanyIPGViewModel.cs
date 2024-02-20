using System.Collections.Generic;
using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;

public class UpdateCompanyIPGViewModel(long id, string providerData,
    List<CreateCompanyIPGDepositViewModel> companyIPGDeposits)
{
    public string ProviderData { get; set; } = providerData;
    public List<CreateCompanyIPGDepositViewModel> CompanyIPGDeposits { get; set; } = companyIPGDeposits;
    public long Id { get; set; } = id;
}
