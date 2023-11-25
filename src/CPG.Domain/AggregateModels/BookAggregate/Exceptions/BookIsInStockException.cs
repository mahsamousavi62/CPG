using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class BookIsInStockException(long bookId) : DomainException(string.Format(Resource.BookNotBorrowed, bookId))
{
    public override string Code => "book_is_not_borrowed";
    public long BookId { get; } = bookId;
}