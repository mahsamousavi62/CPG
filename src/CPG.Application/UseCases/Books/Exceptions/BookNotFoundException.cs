using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Books.Exceptions;

public class BookNotFoundException(long bookId) : ApplicationException($"Book with ID {bookId} has not been found.")
{
    public override string Code => "book_not_found";
    public long BookId { get; } = bookId;
}
