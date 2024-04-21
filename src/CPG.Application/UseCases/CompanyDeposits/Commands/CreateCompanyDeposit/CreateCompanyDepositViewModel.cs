using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;

public class CreateCompanyDepositViewModel
{
    public string Name { get; set; }
    public string Iban { get; set; }
    public long CompanyId { get; set; }
    public string AccountNumber { get; set; }
    public Enums.PaymentMethodType[] MethodTypes { get; set; }
}