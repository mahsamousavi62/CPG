using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Exceptions;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate
{
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
                throw new DaryaftyarUserCreationException($"Parameter {nameof(firstName)} cannot be empty.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DaryaftyarUserCreationException($"Parameter {nameof(lastName)} cannot be empty.");

            FirstName = firstName;
            LastName = lastName;
        }

        public static implicit operator string(Name name) => name.ToString();

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
