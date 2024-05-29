

using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Exceptions;

public class PaymentRequestNotFoundByCodeException : DomainException
{
    public override string Code => "1007001";
    public PaymentRequestNotFoundByCodeException():base(string.Format(Resource.PaymentRequestNotFoundByCode))
    {
        
    }



}
