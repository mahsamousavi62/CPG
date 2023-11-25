using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

internal class BankIsActiveException(long bookId) : DomainException(string.Format(Resource.BankIsAlreadyActive, bookId))
{
    public override string Code => "bank_is_already_active";
    public long BookId { get; } = bookId;
}
