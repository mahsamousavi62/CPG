using System.Collections.Generic;
using CPG.Application.UseCases.CompanyIPGs.ViewModels;

namespace CPG.Application.UseCases.CompanyIPGs.Commands.CreateCompanyIPG;

public class UpdateCompanyIPGViewModel : CreateCompanyIPGViewModel
{
    public UpdateCompanyIPGViewModel(long id,long companyId, long providerId, long iPGTypeId, string providerData,
                                     List<CreateCompanyIPGDepositViewModel> companyIPGDeposits)
                                    : base(companyId, providerId, iPGTypeId, providerData, companyIPGDeposits)
    {
        Id=id;
    }
    public long Id { get; set; }
}
