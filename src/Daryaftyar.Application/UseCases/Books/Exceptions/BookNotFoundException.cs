using ApplicationException = Daryaftyar.Application.UseCases.Exceptions.ApplicationException;

namespace Daryaftyar.Application.UseCases.Books.Exceptions
{
    public class BookNotFoundException : ApplicationException
    {
        public override string Code => "book_not_found";
        public long BookId { get; }

        public BookNotFoundException(long bookId) : base($"Book with ID {bookId} has not been found.") 
            => BookId = bookId;
    }
}
