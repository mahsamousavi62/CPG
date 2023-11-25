using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;

public class NationalCodeInvalidException(string nationalCode) : DomainException($"ISBN nationalCode is wrong. Passed nationalCode  is: {nationalCode}.")
{
    public override string Code => "invalid_natioanalCode";
    public string NationalCode { get; } = nationalCode;
}

