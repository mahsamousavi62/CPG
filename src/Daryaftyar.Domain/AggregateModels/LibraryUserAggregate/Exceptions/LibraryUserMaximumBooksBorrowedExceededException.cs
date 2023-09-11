using Daryaftyar.Domain.Exceptions;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Exceptions
{
    public class DaryaftyarUserMaximumBooksBorrowedExceededException : DomainException
    {
        public override string Code => "cannot_borrow_more_books";

        public DaryaftyarUserMaximumBooksBorrowedExceededException()
            : base("Cannot borrow more books at the same time. User has 3 active loans.")
        {
        }
    }
}