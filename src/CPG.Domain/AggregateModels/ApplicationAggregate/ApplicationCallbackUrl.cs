using CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate;

public class ApplicationCallbackUrl : AuditableEntity<long>
{
    public ApplicationCallbackUrl(string callbackUrl, long applicationId)
    {
        CallbackUrl = callbackUrl;
        ApplicationId = applicationId;
    }

    public ApplicationCallbackUrl(string callbackUrl)
    {
        CallbackUrl = callbackUrl;
    }

    public static List<ApplicationCallbackUrl> Create(Url[] callbackUrlList)
    {
        if (callbackUrlList == null || !callbackUrlList.Any())
            return null;

        if (callbackUrlList.Select(x => x).Distinct().Count() != callbackUrlList.Length)
            throw new DuplicateCallbackUrlException(string.Empty);

        var applicationCallbackUrls = callbackUrlList.Select(i => new ApplicationCallbackUrl(i.Value)).ToList();
        applicationCallbackUrls.ForEach(x => x.IsActive = true);
        return applicationCallbackUrls;
    }

    public string CallbackUrl { get; set; }
    public long ApplicationId { get; set; }
    public Application Application { get; set; }
}