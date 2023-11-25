using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidEnglishNameException(string message) : DomainException(message)
{
    public override string Code => "invalid_EnglishName";
}
