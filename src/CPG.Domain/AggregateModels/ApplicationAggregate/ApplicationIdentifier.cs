using CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;
using CPG.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate;

public class ApplicationIdentifier : AuditableEntity<long>
{
    public ApplicationIdentifier(string idpClientId, long applicationId)
    {
        IdpClientId = idpClientId;
        ApplicationId = applicationId;
    }

    public ApplicationIdentifier(string idpClientId)
    {
        IdpClientId = idpClientId;
    }

    public static List<ApplicationIdentifier> Create(string[] IdpClientIdList)
    {
        if (IdpClientIdList is null || !IdpClientIdList.Any())
            throw new ArgumentNullException(nameof(IdpClientIdList));

        if (IdpClientIdList.Select(x => x).Distinct().Count() != IdpClientIdList.Length)
            throw new DuplicateIdpClientIdException(string.Empty);

        var applicationIdentifiers = IdpClientIdList.Select(i => new ApplicationIdentifier(i)).ToList();
        applicationIdentifiers.ForEach(x => x.IsActive = true);
        return applicationIdentifiers;
    }

    public string IdpClientId { get; set; }
    public long ApplicationId { get; set; }
    public Application Application { get; set; }
}