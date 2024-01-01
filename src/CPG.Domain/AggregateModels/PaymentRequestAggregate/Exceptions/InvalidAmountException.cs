using CPG.Domain.AggregateModels.BankAggregate.Specifications;
using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
