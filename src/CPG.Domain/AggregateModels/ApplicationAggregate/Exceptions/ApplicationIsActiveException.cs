using CPG.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;

public class ApplicationIsActiveException : DomainException
{
    public override string Code => "application_is_already_active";
    public long ApplicationId { get; }

    public ApplicationIsActiveException(long applicationId) : base(string.Format(Resource.ApplicationIsAlreadyActive, applicationId))
       => ApplicationId = applicationId;
}