using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.UserAggregate.Exceptions;
using System.Text.RegularExpressions;

namespace CPG.Domain.SharedKernel;

public class PersianName
{
    public string Value { get; init; }
    public PersianName(string persianName)
    {
        if (string.IsNullOrWhiteSpace(persianName))
            throw new EmptyPersianNameException(persianName);

        if (persianName.Length < 3 || persianName.Length > 255)
            throw new InvalidPersianNameCharachterException(persianName);

        if (!Regex.IsMatch(persianName, "^[\\u0600-\\u06FF\\s]+$"))
            throw new InvalidPersianNameException(persianName);

        Value = persianName;
    }
}
