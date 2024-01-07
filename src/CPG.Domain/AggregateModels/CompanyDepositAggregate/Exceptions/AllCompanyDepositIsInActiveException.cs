using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions
{
    public class AllCompanyDepositsIsInActiveException(long companyId) : DomainException(string.Format(Resource.AllCompanyDepositsInActive, companyId))
    {
        public override string Code => "1001009";
        public long CompanyId { get; } = companyId;
    }
}
