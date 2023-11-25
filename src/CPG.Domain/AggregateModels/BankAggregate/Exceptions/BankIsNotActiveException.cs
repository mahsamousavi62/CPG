using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions
{
    internal class BankIsNotActiveException : DomainException
    {
        public override string Code => "bank_is_already_not_active";
        public long BookId { get; }

        public BankIsNotActiveException(long bookId) : base(string.Format(Resource.BankIsAlreadyNotActive, bookId))
           => BookId = bookId;
    }
}
