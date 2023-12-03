using System;

namespace CPG.Application.UseCases.CompanyDeposits.ViewModels;

public class CompanyDepositViewModel
{
    public long Id { get; set; }
    public int BankId { get; set; }
    public string BankLogo { get; set; }
    public string BankName { get; set; }
    public long CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string AccountNumber { get; set; }
    public string Name { get; set; }
    public string Iban { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
}
