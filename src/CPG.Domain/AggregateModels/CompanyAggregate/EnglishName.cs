using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using System.Text.RegularExpressions;

namespace CPG.Domain.AggregateModels.CompanyAggregate
{
    public class EnglishName
    {
        public string Value { get; init; }

        public EnglishName(string englishName)
        {
            if (string.IsNullOrWhiteSpace(englishName))
                throw new EmptyEnglishNameException($"Parameter {nameof(englishName)} cannot be empty.");

            if (!Regex.IsMatch(englishName,"[A-Za-z\\s]+"))

                throw new InvalidEnglishNameException($"Parameter {nameof(englishName)} is invalid.");
           
            Value = englishName;
        }
    }
}
