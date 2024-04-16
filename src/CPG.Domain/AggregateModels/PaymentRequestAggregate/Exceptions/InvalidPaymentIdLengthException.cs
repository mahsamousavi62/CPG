using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Exceptions;

public class InvalidPaymentIdLengthException() : DomainException(Resource.InvalidPaymentIdLength)
{
    public override string Code => "invalid_paymentIdentifier_length";
}