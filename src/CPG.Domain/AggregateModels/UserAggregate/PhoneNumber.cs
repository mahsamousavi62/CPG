using Ardalis.GuardClauses;

namespace CPG.Domain.AggregateModels.UserAggregate
{
    public record PhoneNumber
    {
        public string Value { get; }

        public PhoneNumber(string phoneNumber)
        {
            Guard.Against.NullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            //if (phoneNumber.Length != 10 || !Regex.IsMatch(phoneNumber, "^\\d{10}$"))
            //    throw new PhoneNumberInvalidFormatException(phoneNumber);

            Value = phoneNumber;
        }
        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
        public static implicit operator PhoneNumber(string phoneNumber) => new(phoneNumber);

        public override string ToString() => Value;
    }
}
