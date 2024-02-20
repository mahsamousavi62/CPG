namespace CPG.Application.UseCases.DirectDebit.ViewModels;

public class BankPlanViewModel
{
    public int PlanId { get; set; }
    public decimal MaxWithdrawalAmountPerDay { get; set; }
    public short DurationPerMonth { get; set; }
}
