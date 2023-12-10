using Ardalis.GuardClauses;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate
{
    public class Amount
    {
        public decimal Value { get; }

        private Amount()
        {
        }
        public Amount(decimal amount)
        {
            decimal minValue = 10000m;
            decimal maxValue = 100000000000m;

            if (amount < minValue || amount > maxValue)
                throw new InvalidAmountException(amount);
        }

        public static implicit operator decimal(Amount amount) => amount.Value;
        public static implicit operator Amount(decimal value) => new(value);

        public override string ToString() => Value.ToString();


    }
}
