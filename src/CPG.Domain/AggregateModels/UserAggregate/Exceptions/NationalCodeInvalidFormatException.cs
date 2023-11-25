using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class NationalCodeInvalidFormatException(string nationalCode) : DomainException($"NationalCode value has to be 10 length. Passed NationalCode is: {nationalCode}.")
{
    public override string Code => "invalid_nationalCode_format";
    public string NationalCode { get; } = nationalCode;
}
