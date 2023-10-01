using Daryaftyar.Domain.Exceptions;

namespace Daryaftyar.Domain.AggregateModels.BookAggregate.Exceptions
{
    public class BookIsNotInStockException : DomainException
    {
        public override string Code => "book_is_already_borrowed";

        public BookIsNotInStockException() : base(Resource.BookIsAlreadyBorrowed)
        {
        }
    }
}
