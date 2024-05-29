using CPG.Domain.Exceptions;


namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Exceptions;


public class PaymentRequestCodeIsUsedBeforeException : DomainException
{
    public PaymentRequestCodeIsUsedBeforeException() : base(string.Format(Resource.PaymentRequestCodeIsUsedBefore))
    {
    }

    public override string Code => "1007003";
}