using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions
{
    public class NationalCodeInvalidFormatException : DomainException
    {
        public override string Code => "invalid_nationalCode_format";
        public string NationalCode { get; }

        public NationalCodeInvalidFormatException(string nationalCode) : base($"NationalCode value has to be 10 length. Passed NationalCode is: {nationalCode}.") 
            => NationalCode =nationalCode;
    }
}
