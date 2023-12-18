using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate.Exceptions;

public class MultipleDefaultDepositException(string name) : DomainException(string.Format(Resource.MultipleDefaultDeposit))
{
    public override string Code => "multiple_default_Deposit";

}