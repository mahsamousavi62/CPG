using Ardalis.Specification;
using System;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

public class ApplicationCallbackUrlsByAppId : Specification<ApplicationCallbackUrl, ISingleResultSpecification<ApplicationCallbackUrl>>
{
    public ApplicationCallbackUrlsByAppId(string[] callbackUrls)
    {
        Query
            .Where(app => callbackUrls.Contains(app.CallbackUrl));
    }
}