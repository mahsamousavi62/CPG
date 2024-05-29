using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Exceptions;

public class PaymentRequestCodeInvalidStatusException : DomainException
{
    public PaymentRequestCodeInvalidStatusException(string message) : base(string.Format(Resource.PaymentRequestCodeInvalidStatus))
    {
    }

    public override string Code => "1007004";
}
