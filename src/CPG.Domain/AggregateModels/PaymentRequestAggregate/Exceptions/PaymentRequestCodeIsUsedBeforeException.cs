using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Exceptions;


public class PaymentRequestCodeIsUsedBeforeException : DomainException
{
    public PaymentRequestCodeIsUsedBeforeException() : base(string.Format(Resource.PaymentRequestCodeIsUsedBefore))
    {
    }

    public override string Code => "1007003";
}