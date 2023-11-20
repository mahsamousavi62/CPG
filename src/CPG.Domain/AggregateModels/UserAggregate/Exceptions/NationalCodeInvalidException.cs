using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions
{
    public class NationalCodeInvalidException : DomainException
    {
        public override string Code => "invalid_natioanalCode";
        public string NationalCode { get; }

        public NationalCodeInvalidException(string nationalCode) : base($"ISBN nationalCode is wrong. Passed nationalCode  is: {nationalCode}.")
            => NationalCode = nationalCode;
    }
}

