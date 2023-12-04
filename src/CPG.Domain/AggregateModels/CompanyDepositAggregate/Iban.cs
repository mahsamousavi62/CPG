using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BankAggregate.Guards;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CPG.Domain.AggregateModels.BankAggregate;

public record Iban
{
    public string Value { get; }

    private Iban()
    {
    }

    public Iban(string iban)
    {
        iban = iban.Trim().ToUpper();
        Guard.Against.NullOrWhiteSpace(iban, nameof(iban));

        if (iban.Length != 26)
            throw new IbanInvalidFormatException(iban);

        if (!iban[..2].ToCharArray().All(t => char.IsLetter(t)) || iban[..2] != "IR")
            throw new IbanInvalidFormatException(iban);

        if (!Validate(iban))
            throw new IbanInvalidException(iban);

        Value = iban;
    }

    public static implicit operator string(Iban iban) => iban.Value;
    public static implicit operator Iban(string value) => new(value);

    public override string ToString() => Value;

    bool Validate(string iban)
    {
        var changediban = iban.Substring(4, 22);
        changediban = changediban.Insert(22, (Convert.ToInt16(iban[0]) - 55).ToString());
        changediban = changediban.Insert(24, (Convert.ToInt16(iban[1]) - 55).ToString());
        changediban = changediban.Insert(26, iban.Substring(2, 2));
        return decimal.Parse(changediban) % 97 == 1;
    }
}
