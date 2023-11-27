using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BankAggregate.Guards;
using System.Text.RegularExpressions;

namespace CPG.Domain.AggregateModels.BankAggregate;

public record IbanPrefix
{
    public string Value { get; }

    private IbanPrefix()
    {
    }

    public IbanPrefix(string IbanPrefix)
    {
        Guard.Against.NullOrWhiteSpace(IbanPrefix, nameof(IbanPrefix));

        const string nonDigitsPattern = "[^.0-9]";
        IbanPrefix = Regex.Replace(IbanPrefix, nonDigitsPattern, string.Empty);

        Value = Guard.Against.IbanPrefixCorrectness(IbanPrefix, nameof(IbanPrefix));
    }

    public static implicit operator string(IbanPrefix IbanPrefix) => IbanPrefix.Value;
    public static implicit operator IbanPrefix(string value) => new(value);

    public override string ToString() => Value;
}
