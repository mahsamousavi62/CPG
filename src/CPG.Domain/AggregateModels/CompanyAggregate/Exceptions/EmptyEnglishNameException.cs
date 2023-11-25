using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class EmptyEnglishNameException(string message) : DomainException(message)
{
    public override string Code => "empty_englishName";
}
