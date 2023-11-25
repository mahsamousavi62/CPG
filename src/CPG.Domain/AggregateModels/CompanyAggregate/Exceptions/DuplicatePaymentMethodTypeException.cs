using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class DuplicatePaymentMethodTypeException : DomainException
{
    public override string Code => "duplicate_PaymentMethodType";
    public DuplicatePaymentMethodTypeException(string message) : base(message)
    {
    }

}
