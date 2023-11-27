using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.SharedKernel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class EnglishName
{
    private readonly IAggregateRepository<Company> _repository;
    public string Value { get; init; }
    public EnglishName(IAggregateRepository<Company> repository)
    {
        _repository = repository;
    }

        public EnglishName(string englishName)
        {
            if (string.IsNullOrWhiteSpace(englishName))
                throw new EmptyEnglishNameException($"Parameter {nameof(englishName)} cannot be empty.");

        if (!Regex.IsMatch(englishName, "[A-Za-z\\s]+"))

            throw new InvalidEnglishNameException($"Parameter {nameof(englishName)} is invalid.");

       // Task.Run(() => CheckUniqueName(englishName)).Wait();

        Value = englishName;
    }

    private async Task CheckUniqueName(string name)
    {
        Company company = await _repository.GetBySpecAsync(new CompanyByPersianName(name));

        if (company != null) throw new DuplicatePersianNameException(name);
    }
}
