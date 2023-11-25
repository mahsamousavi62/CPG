using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Banks.ViewModels;

public class BankViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public BankStatus Status { get; set; }

    public string SwiftCode { get; set; }

    public int? ProviderId { get; set; }

    public byte[] Logo { get; set; }

    public string ProviderData { get; set; }

    public decimal? DirectDebitAmountLimit { get; set; }

    public decimal? DirectDebitDailyTransactionLimit { get; set; }
}
