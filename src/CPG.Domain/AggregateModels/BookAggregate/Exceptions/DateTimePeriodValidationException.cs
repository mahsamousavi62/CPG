using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class DateTimePeriodValidationException(string message) : DomainException(message)
{
    public override string Code => "Datetime period validation failed.";
}
