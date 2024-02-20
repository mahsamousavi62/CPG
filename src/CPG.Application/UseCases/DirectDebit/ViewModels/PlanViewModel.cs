using CPG.Application.UseCases.Banks.ViewModels;
using System.Collections.Generic;

namespace CPG.Application.UseCases.DirectDebit.ViewModels;

public class PlanViewModel
{
    public IReadOnlyCollection<BankPlanViewModel> BankPlans { get; set; }
    public int DirectDebitGrantCount { get; set; }
    public BankDataViewModel Bank { get; set; }
    public IReadOnlyCollection<GrantDataViewModel> GrantDetails { get; set; }
}
