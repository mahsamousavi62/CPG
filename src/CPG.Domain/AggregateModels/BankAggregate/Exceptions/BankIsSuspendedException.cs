using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

internal class BankIsSuspendedException(long bookId) : DomainException(string.Format(Resource.BankIsAlreadySuspended, bookId))
{
    public override string Code => "bank_is_already_suspended";
    public long BookId { get; } = bookId;
}
