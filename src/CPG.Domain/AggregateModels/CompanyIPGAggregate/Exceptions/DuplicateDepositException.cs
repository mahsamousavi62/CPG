using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate.Exceptions;

public class DuplicateDepositException(string name) : DomainException(string.Format(Resource.DuplicateDeposit))
{
    public override string Code => "duplicate_deposit";

}