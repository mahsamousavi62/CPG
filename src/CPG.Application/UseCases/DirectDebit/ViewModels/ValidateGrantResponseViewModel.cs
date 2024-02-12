using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.DirectDebit.ViewModels;

public class ValidateGrantResponseViewModel
{
    public DirectDebitGrantStatus Status { get; set; }
    public string BankLogo { get; set; }
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
    public decimal? MaxWithdrawalAmountPerDay { get; set; }
    public short? DurationPerMonth { get; set; }
}
