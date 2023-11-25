using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class BookAlreadyExistsInStorageException(long bookId) : DomainException(string.Format(Resource.BookAlreadyExistsInTheStorage, bookId))
{
    public override string Code => "book_already_exists_in_the_storage";
    public long BookId { get; } = bookId;
}
