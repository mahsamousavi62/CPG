using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.UserAggregate.Exceptions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel;

public class PersianName
{
    private readonly IAggregateRepository<Company> _repository;
    public PersianName(IAggregateRepository<Company> repository)
    {
        _repository = repository;
    }
    public string Value { get; init; }

    public PersianName(string persianName)
    {
        Guard.Against.NullOrEmpty(persianName);

        if (string.IsNullOrWhiteSpace(persianName))
            throw new UserCreationException($"Parameter {nameof(persianName)} cannot be empty.");



        if (!Regex.IsMatch(persianName, "^[\\u0600-\\u06FF\\s]+$"))
            throw new InvalidPersianNameException(persianName);

        //Task.Run(() => CheckUniqueName(persianName)).Wait();

        Value = persianName;
    }


    private async Task CheckUniqueName(string name)
    {
        Company company = await _repository.GetBySpecAsync(new CompanyByPersianName(name));

        if (company != null) throw new DuplicatePersianNameException(name);
    }
}
