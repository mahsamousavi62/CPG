using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidEnglishNameException(string message) : DomainException(string.Format(Resource.Invalid_EnglishName, message))
{
    public override string Code => "invalid_EnglishName";

    public string Message { get; } = message;
}
