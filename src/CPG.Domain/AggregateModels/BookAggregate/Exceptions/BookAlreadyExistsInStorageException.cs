using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions
{
    public class BookAlreadyExistsInStorageException : DomainException
    {
        public override string Code => "book_already_exists_in_the_storage";
        public long BookId { get; }

        public BookAlreadyExistsInStorageException(long bookId) : base(string.Format(Resource.BookAlreadyExistsInTheStorage, bookId)) 
            => BookId = bookId;
    }
}
