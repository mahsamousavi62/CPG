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

            switch (phoneNumber.Length)
            {
                case 10 when phoneNumber.StartsWith("9"):
                    Value = "0" + phoneNumber;
                    break;
                case 11 when phoneNumber.StartsWith("09"):
                    Value = phoneNumber;
                    break;
                case 12 when phoneNumber.StartsWith("98"):
                    Value = "0" + phoneNumber[2..];
                    break;
                default:
                    throw new PhoneNumberInvalidFormatException(phoneNumber);
            }
        }
        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
        public static implicit operator PhoneNumber(string phoneNumber) => new(phoneNumber);

        public override string ToString() => Value;
    }
}
