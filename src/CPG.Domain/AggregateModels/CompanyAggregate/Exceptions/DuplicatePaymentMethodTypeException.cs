using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class DuplicatePaymentMethodTypeException : DomainException
    {
        public override string Code => "duplicate_PaymentMethodType";
        public DuplicatePaymentMethodTypeException(string message) : base(message)
        {
        }

    }
}
