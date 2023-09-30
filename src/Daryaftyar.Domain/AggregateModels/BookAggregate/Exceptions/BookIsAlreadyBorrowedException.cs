using Daryaftyar.Domain.Exceptions;

namespace Daryaftyar.Domain.AggregateModels.BookAggregate.Exceptions
{
    public class BookIsAlreadyBorrowedException : DomainException
    {
        public override string Code => "book_is_already_borrowed";

        public BookIsAlreadyBorrowedException() : base(Resource.BookIsAlreadyBorrowed)
        {
        }
    }
}