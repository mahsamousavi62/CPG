using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class BookNotBorrowedForUserException(long bookId) : DomainException($"Book with ID {bookId} is not borrowed by given user.")
{
    public override string Code => "book_not_borrowed_by_given_user";
    public long BookId { get; } = bookId;
}