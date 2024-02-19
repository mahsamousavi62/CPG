using CPG.Domain.AggregateModels.UserAggregate.Exceptions;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BookAggregate.Exceptions;

namespace CPG.Domain.AggregateModels.UserAggregate;

public record NationalCode
{
    public string Value { get; init; }

    public NationalCode(string nationalCode, bool isLegal = false)
    {
        Guard.Against.NullOrWhiteSpace(nationalCode, nameof(nationalCode));

        if (isLegal)
        {
            if (nationalCode.Length != 11 || !Regex.IsMatch(nationalCode, "^\\d{11}$"))
                throw new NationalCodeInvalidFormatException(nationalCode);
        }
        else
        {
            if (nationalCode.Length != 10 || !Regex.IsMatch(nationalCode, "^\\d{10}$"))
                throw new NationalCodeInvalidFormatException(nationalCode);

            if (!IsValid(nationalCode))
                throw new NationalCodeInvalidException(nationalCode);
        }
        Value = nationalCode;
    }


    private static bool IsValid(string nationalCode)
    {
        var check = Convert.ToInt32(nationalCode.Substring(9, 1));
        var sum = Enumerable.Range(0, 9)
                      .Select(x => Convert.ToInt32(nationalCode.Substring(x, 1)) * (10 - x))
                      .Sum() % 11;

        return sum < 2 && check == sum || sum >= 2 && check + sum == 11;
    }

    public static implicit operator string(NationalCode nationalCode) => nationalCode.Value;
    public static implicit operator NationalCode(string nationalCode) => new(nationalCode);

    public override string ToString() => Value;
}
