using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyIPGAggregate.Exceptions;

public class NotFoundDepositException(string idList) : DomainException(string.Format(Resource.NotFoundDeposit, idList))
{
    public override string Code => "notFound_deposits";
}