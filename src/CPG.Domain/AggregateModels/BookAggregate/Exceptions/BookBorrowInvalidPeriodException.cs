using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class BookBorrowInvalidPeriodException(int requestedLoanPeriodInDays, int maximumAvailableLoanPeriodInDays) : DomainException($"Cannot borrow book for {requestedLoanPeriodInDays} days. The maximum period that book can be borrowed is {maximumAvailableLoanPeriodInDays} days.")
{
    public override string Code => "invalid_days_period_for_borrowing_book";
    public int DaysPeriod { get; } = requestedLoanPeriodInDays;
    public int MaximumAvailableLoanPeriodInDays { get; } = maximumAvailableLoanPeriodInDays;
}
