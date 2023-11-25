using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidPersianNameException(string message) : DomainException(string.Format(Resource.Invalid_PersianName, message))
{
    public override string Code => "invalid_persianName";

    public string Message { get; } = message;

}
