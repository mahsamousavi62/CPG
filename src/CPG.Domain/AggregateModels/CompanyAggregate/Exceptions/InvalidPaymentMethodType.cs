using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class InvalidPaymentMethodType(string message)
        : DomainException(string.Format(Resource.Invalid_PaymentMethodType, message))
{
    public override string Code => "invalid_PaymentMethodType";
}
