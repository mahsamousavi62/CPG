

using CPG.Domain.Exceptions;
using System;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Exceptions;

public class PaymentRequestCodeExpiredException : DomainException
{
    public override string Code => "1007002";
    public PaymentRequestCodeExpiredException() : base(string.Format(Resource.PaymentRequestCodeExpired))
    {
    }
}
