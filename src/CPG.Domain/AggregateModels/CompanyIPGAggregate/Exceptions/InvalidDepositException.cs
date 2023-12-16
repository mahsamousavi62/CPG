using CPG.Domain.Exceptions;


namespace CPG.Domain.AggregateModels.CompanyIPGAggregate.Exceptions;

public class InvalidDepositsException(string idList, long companyId) : DomainException(string.Format(Resource.InvalidDeposits, idList, companyId))
{
    public override string Code => "invalid_deposits";
}