using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;

public class ApplicationIsNotActiveException : DomainException
{
    public override string Code => "application_is_already_not_active";
    public long ApplicationId { get; }

    public ApplicationIsNotActiveException(long applicationId) : base(string.Format(Resource.ApplicationIsAlreadyNotActive, applicationId))
       => ApplicationId = applicationId;
}
