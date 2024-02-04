using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

public class InvalidDDBankCodeExceptionn(decimal ddBankCode) : DomainException(Resource.InvalidDDBankCode)
{
    public override string Code => "invalid_ddBankCode";
}