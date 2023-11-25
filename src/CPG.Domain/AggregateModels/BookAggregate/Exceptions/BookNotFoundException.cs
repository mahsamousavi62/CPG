using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class BookNotFoundException(long bookId) : DomainException($"Book with ID {bookId} was not found.")
{
    public override string Code => "book_not_found";
    public long BookId { get; } = bookId;
}
