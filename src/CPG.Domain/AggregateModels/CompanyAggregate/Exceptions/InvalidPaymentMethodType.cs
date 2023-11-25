using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

internal class InvalidPaymentMethodType : DomainException
{
    public override string Code => "invalid_PaymentMethodType_extention";
    public InvalidPaymentMethodType(string message) : base(message)
    {
    }
}
