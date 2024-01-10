using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidKeyFormatException(string message) : DomainException(string.Format(Resource.InvalidKeyFormat, message))
{
    public override string Code => "invalid_key_format";
}
