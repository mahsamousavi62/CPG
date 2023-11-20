using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions
{
    public class PhoneNumberInvalidFormatException : DomainException
    {
        public override string Code => "invalid_PhoneNumber_format";
        public string PhoneNumber { get; }

        public PhoneNumberInvalidFormatException(string phoneNumber) : base($"PhoneNumber value has to be 10 length. Passed PhoneNumber is: {phoneNumber}.") 
            => PhoneNumber =phoneNumber;
    }
}
