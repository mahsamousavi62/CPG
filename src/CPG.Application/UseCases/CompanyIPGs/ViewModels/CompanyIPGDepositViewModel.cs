using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.CompanyIPGs.ViewModels;

public class CompanyIPGDepositViewModel
{
    public long Id { get; set; }
    public long CompanyIPGId { get; set; }
    public long CompanyDepositId { get; set; }
    public bool IsDefault { get; set; }
    public CompanyIPG CompanyIPG { get; set; }
    public CompanyDeposit CompanyDeposit { get; set; }
}
