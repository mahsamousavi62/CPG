using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BookAggregate.Exceptions;

namespace CPG.Domain.AggregateModels.UserAggregate
{
    public record PhoneNumber
    {
        public string Value { get; }

        public PhoneNumber(string phoneNumber)
        {
            Guard.Against.NullOrWhiteSpace(phoneNumber, nameof(phoneNumber));

            if (phoneNumber.Length != 12 || !Regex.IsMatch(phoneNumber, "^\\d{12}$"))
                throw new PhoneNumberInvalidFormatException(phoneNumber);

            var formatNumber = "0" + phoneNumber[2..];
            Value = formatNumber;
        }
        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
        public static implicit operator PhoneNumber(string phoneNumber) => new(phoneNumber);

        public override string ToString() => Value;
    }
}
