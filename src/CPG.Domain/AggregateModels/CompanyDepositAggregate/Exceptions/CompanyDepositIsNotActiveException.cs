using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions;

public class CompanyDepositIsNotActiveException(long companyId) : DomainException(string.Format(Resource.CompanyDepositIsAlreadyNotActive, companyId))
{
    public override string Code => "1001006";
    public long CompanyId = companyId;
}
