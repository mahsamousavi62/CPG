using CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate;

public record Name
{
    public string FirstName { get; }
    public string LastName { get; }

    public Name()
    {
    }

    public Name(string firstName, string lastName)
    {
        // TODO: Implement Guard clause
        if (string.IsNullOrWhiteSpace(firstName))
            throw new CPGUserCreationException($"Parameter {nameof(firstName)} cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new CPGUserCreationException($"Parameter {nameof(lastName)} cannot be empty.");

        FirstName = firstName;
        LastName = lastName;
    }

    public static implicit operator string(Name name) => name.ToString();

    public override string ToString() => $"{FirstName} {LastName}";
}
