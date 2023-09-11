using Daryaftyar.Domain.Exceptions;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Exceptions
{
    public class DaryaftyarUserDoesNotHaveBookBorrowed : DomainException
    {
        public override string Code => "user_does_not_have_this_book_borrowed";

        public DaryaftyarUserDoesNotHaveBookBorrowed(long bookId)
            : base($"User does not have this book borrowed in the system.")
        {
        }
    }
}