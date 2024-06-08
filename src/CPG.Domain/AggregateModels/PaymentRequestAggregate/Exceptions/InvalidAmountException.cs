using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Exceptions
{
    public class InvalidAmountException: DomainException
    {
        public override string Code => "amount_is_not_in_range";
        public decimal Amount { get; }

        public InvalidAmountException(decimal amount) : base(string.Format(Resource.AmountIsNotInRange, amount))
           => Amount = amount;
    }
}
