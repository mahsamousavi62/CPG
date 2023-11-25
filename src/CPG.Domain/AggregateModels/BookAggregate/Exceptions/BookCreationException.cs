using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class BookCreationException(string message) : DomainException(message)
{
    public override string Code => "cannot_create_book";
}
