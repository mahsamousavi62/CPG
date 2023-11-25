using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.Books.Exceptions;

public class BookNotAvailableException(long bookId) : ApplicationException($"Book is already borrowed.")
{
    public long BookId { get; } = bookId;
    public override string Code => "book_is_already_borrowed";
}