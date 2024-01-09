using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidIvCharachterException(string message) : DomainException(string.Format(Resource.InvalidIvCharachter, message))
{
    public override string Code => "invalid_iv_charachter";
}
