using CPG.Domain.SharedKernel.File;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;

public class CreateCompanyDepositViewModel
{
    public string Name { get; set; }
    public string Iban { get; set; }
   
}