using CPG.Domain.SharedKernel.File;
using System;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyDeposits.ViewModels;

public class CompanyDepositViewModel
{
    public long Id { get; set; }
    public  string BankLogo { get; set; }
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
    public string Name { get; set; }
    public string Iban { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }
}
