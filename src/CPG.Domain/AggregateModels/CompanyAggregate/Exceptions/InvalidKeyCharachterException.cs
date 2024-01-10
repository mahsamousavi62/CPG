using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidKeyCharachterException(string message) : DomainException(string.Format(Resource.InvalidKeyCharachter, message))
{
    public override string Code => "invalid_key_charachter";
}