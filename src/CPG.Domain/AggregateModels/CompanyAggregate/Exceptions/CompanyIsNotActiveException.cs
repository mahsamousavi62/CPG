using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

public class CompanyIsNotActiveException : DomainException
{
    public override string Code => "company_is_already_not_active";
    public long CompanyId { get; }

    public CompanyIsNotActiveException(long companyId) : base(string.Format(Resource.CompanyIsAlreadyNotActive, companyId))
       => CompanyId = companyId;
}
