using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

internal class InvalidIVFormatException(string message) : DomainException(string.Format(Resource.InvalidIvFormat, message))
{
    public override string Code => "invalid_iv_format";
}