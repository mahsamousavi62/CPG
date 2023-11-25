using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class PhoneNumberInvalidFormatException(string phoneNumber) : DomainException($"PhoneNumber value has to be 10 length. Passed PhoneNumber is: {phoneNumber}.")
{
    public override string Code => "invalid_PhoneNumber_format";
    public string PhoneNumber { get; } = phoneNumber;
}
