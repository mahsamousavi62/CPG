using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

internal class BankIsNotActiveException(long bookId) : DomainException(string.Format(Resource.BankIsAlreadyNotActive, bookId))
{
    public override string Code => "bank_is_already_not_active";
    public long BookId { get; } = bookId;
}
