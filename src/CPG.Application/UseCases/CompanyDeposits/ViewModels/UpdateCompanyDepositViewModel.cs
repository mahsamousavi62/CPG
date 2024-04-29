using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.CompanyDeposits.ViewModels;

public class UpdateCompanyDepositViewModel
{
    public string Name { get; set; }
    public long Id { get; set; }
    public Enums.PaymentMethodType[] methodTypes { get; set; }
}
