using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions
{
    public class CPGUserDoesNotHaveBookBorrowed : DomainException
    {
        public override string Code => "user_does_not_have_this_book_borrowed";

        public CPGUserDoesNotHaveBookBorrowed(long bookId)
            : base($"User does not have this book borrowed in the system.")
        {
        }
    }
}