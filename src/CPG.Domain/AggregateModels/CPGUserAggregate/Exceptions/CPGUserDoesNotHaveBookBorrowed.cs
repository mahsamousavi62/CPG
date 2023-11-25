using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;

public class CPGUserDoesNotHaveBookBorrowed(long bookId) : DomainException($"User does not have this book borrowed in the system.")
{
    public override string Code => "user_does_not_have_this_book_borrowed";
}