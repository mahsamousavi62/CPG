using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class DuplicatePaymentMethodTypeException(string name) 
    : DomainException(string.Format(Resource.Duplicate_PaymentMethodType))
{
    public override string Code => "duplicate_PaymentMethodType";
    
}
