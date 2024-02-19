using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.UserAggregate.Exceptions;

namespace CPG.Domain.AggregateModels.UserAggregate;

public record Name
{
    public string FirstName { get; }
    public string LastName { get; }

    public Name()
    {
    }

    public Name(string firstName, string lastName, bool isLegal = false)
    {
        Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
        if (!isLegal)
        {
            Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
        }
        FirstName = firstName;
        LastName = lastName;
    }

    public static implicit operator string(Name name) => name.ToString();

    public override string ToString() => $"{FirstName} {LastName}";
}
