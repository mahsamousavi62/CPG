using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel;

public class EnglishName
{
    public string Value { get; init; }

    public EnglishName(string englishName)
    {
        if (string.IsNullOrWhiteSpace(englishName))
            throw new EmptyEnglishNameException(englishName);

        if (!Regex.IsMatch(englishName, "[A-Za-z\\s]+"))
            throw new InvalidEnglishNameException(englishName);

        Value = englishName;
    }

}
