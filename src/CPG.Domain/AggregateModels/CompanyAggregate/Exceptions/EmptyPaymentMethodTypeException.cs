using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyPaymentMethodTypeException(string name)
        : DomainException(string.Format(Resource.Empty_PaymentMethodType, name))
    {
        public override string Code => "empty_paymentmethodtype";
    }
}
