using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class CompanyHasNotCompanyDepositException(long companyId) :
        DomainException(string.Format(Resource.CompanyHasNotCompanyDeposit, companyId))
    {
        public override string Code => "Company_HasNotCompanyDeposit";
        public long CompanyId { get; } = companyId;

    }
}
