using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using System.Text.RegularExpressions;

namespace CPG.Domain.SharedKernel;

public class EnglishName
{
    public string Value { get; init; }

    public EnglishName(string englishName)
    {
        if (string.IsNullOrWhiteSpace(englishName))
            throw new EmptyEnglishNameException(englishName);

        if (englishName.Length<3 || englishName.Length>255)
            throw new InvalidEnglishNameCharachterException(englishName);

        if (!Regex.IsMatch(englishName, "[A-Za-z\\s]+"))
            throw new InvalidEnglishNameException(englishName);

        Value = englishName;
    }
}
