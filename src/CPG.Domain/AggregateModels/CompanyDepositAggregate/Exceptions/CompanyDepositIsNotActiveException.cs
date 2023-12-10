using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions;

public class CompanyDepositIsNotActiveException : DomainException
{
    public override string Code => "companyDeposit_is_already_not_active";
    public long CompanyId { get; }

    public CompanyDepositIsNotActiveException(long companyId) : base(string.Format(Resource.CompanyDepositIsAlreadyNotActive, companyId))
       => CompanyId = companyId;
}
