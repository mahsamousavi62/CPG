

using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class PaymentRequestNotFoundByCodeException : DomainException
{
    public override string Code => "1007001";
    public PaymentRequestNotFoundByCodeException():base(string.Format(Resource.PaymentRequestNotFoundByCode))
    {
        
    }



}
