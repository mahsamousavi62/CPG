using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BookAggregate.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace CPG.Domain.AggregateModels.BookAggregate.Guards
{
    public static partial class IsbnCorrectnessExtension
    {
        private static readonly Regex Isbn10FormatPattern = Isbn10Format();
        private static readonly Regex Isbn13FormatPattern = Isbn13Format();
        
        public static string IsbnCorrectness(this IGuardClause guardClause, [MaybeNull] string input, string parameterName, string message = null)
        {
            if (input != null && !Isbn10FormatPattern.IsMatch(input) && !Isbn13FormatPattern.IsMatch(input))
                throw new BookIsbnInvalidFormatException(input);

            return input;
        }

        [GeneratedRegex(@"^(?:ISBN(?:-10)?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$)[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$")]
        private static partial Regex Isbn10Format();

        [GeneratedRegex(@"^(?:ISBN(?:-13)?:? )?(?=[0-9]{13}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)97[89][- ]?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9]$")]
        private static partial Regex Isbn13Format();
    }
}