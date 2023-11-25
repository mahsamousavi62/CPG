using Ardalis.GuardClauses;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.BankAggregate;

public record BankInformation
{
    public string Name { get; set; }

    public string SwiftCode { get; set; }

    public BankStatus Status { get; set; }

    public byte[] Logo { get; set; }

    public int? ProviderId { get; set; }

    public string ProviderData { get; set; }

    public DirectDebitLimitation DirectDebitLimitation { get; set; }

    private BankInformation()
    {
    }

    public BankInformation(string name, string swiftCode, BankStatus status, byte[] logo, int? providerId,
                           string providerData, decimal amount, decimal dailyTransaction)
    {
        Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Status = Guard.Against.Null(status, nameof(status));
        ProviderId = providerId != null ? Guard.Against.NegativeOrZero((int)providerId, nameof(providerId)) : providerId;
        ProviderData = providerId != null ? Guard.Against.NullOrWhiteSpace(providerData, nameof(providerData)) : providerData;
        DirectDebitLimitation = new DirectDebitLimitation(amount, dailyTransaction);
    }
}
