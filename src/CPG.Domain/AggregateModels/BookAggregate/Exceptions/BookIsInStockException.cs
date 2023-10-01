using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions
{
    public class BookIsInStockException : DomainException
    {
        public override string Code => "book_is_not_borrowed";
        public long BookId { get; }

        public BookIsInStockException(long bookId) : base(string.Format(Resource.BookNotBorrowed, bookId))
            => BookId = bookId;
    }
}