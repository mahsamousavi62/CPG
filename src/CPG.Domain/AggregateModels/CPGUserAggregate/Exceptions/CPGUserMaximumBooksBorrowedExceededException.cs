using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;

public class CPGUserMaximumBooksBorrowedExceededException : DomainException
{
    public override string Code => "cannot_borrow_more_books";

    public CPGUserMaximumBooksBorrowedExceededException()
        : base("Cannot borrow more books at the same time. User has 3 active loans.")
    {
    }
}