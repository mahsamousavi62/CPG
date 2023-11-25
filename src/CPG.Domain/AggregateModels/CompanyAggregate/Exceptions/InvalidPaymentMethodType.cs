using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

internal class InvalidPaymentMethodType : DomainException
{
    public override string Code => "invalid_PaymentMethodType_extention";
    public InvalidPaymentMethodType(string message) : base(message)
    {
    }
}
