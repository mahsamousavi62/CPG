using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CPG.Domain.AggregateModels.BankAggregate.Guards;

public static partial class IbanPrefixCorrectnessExtension
{
    private static readonly Regex IbanPrefixFormatPattern = IbanPrefixFormat();    

    public static string IbanPrefixCorrectness(this IGuardClause guardClause, [MaybeNull] string input, string parameterName, string message = null)
    {
        if (input != null && !IbanPrefixFormatPattern.IsMatch(input))
            throw new IbanPrefixInvalidFormatException(input);

        return input;
    }

    [GeneratedRegex(@"^\d{3}")]
    private static partial Regex IbanPrefixFormat();
}