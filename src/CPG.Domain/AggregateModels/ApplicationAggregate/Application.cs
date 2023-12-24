using CPG.Domain.AggregateModels.ApplicationAggregate.Events;
using CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.ApplicationAggregate;

public class Application : AuditableEntity<long>, IAggregateRoot
{
    public Application()
    {

    }
    public Application(PersianName persianName, EnglishName englishName, Logo logo, Url responseApiUrl)
    {
        PersianName = persianName.Value;
        EnglishName = englishName.Value;
        Logo = logo.Value;
        ResponseApiUrl = responseApiUrl.Value;
        ApplicationIdentifiers = [];
    }

    public string PersianName { get; }

    public string EnglishName { get; }

    public string Logo { get; }

    public string ResponseApiUrl { get; }

    public List<ApplicationIdentifier> ApplicationIdentifiers { get; set; } = [];

    public List<ApplicationCallbackUrl> ApplicationCallbackUrls { get; set; } = [];

    public List<PaymentRequest> PaymentRequests { get; set; } = [];

    public static Application Create(PersianName persianName, EnglishName englishName, Url responseApiUrl, Logo logo, string[] details, Url[] callbackUrls)
    {
        var application = new Application(persianName, englishName, logo, responseApiUrl);

        var applicationIdentifiers = ApplicationIdentifier.Create(details);
        application.ApplicationIdentifiers.AddRange(applicationIdentifiers);

        if (callbackUrls?.Length > 0 is true)
        {
            var applicationCallbackUrls = ApplicationCallbackUrl.Create(callbackUrls);
            application.ApplicationCallbackUrls.AddRange(applicationCallbackUrls);
        }
        application.IsActive = true;        

        return application;
    }

    public void SetAsActive()
    {
        if (IsActive == true)
            throw new ApplicationIsActiveException(Id);

        IsActive = true;

        AddDomainEvent(new ChangeApplicationStatusEvent(Id, IsActive, DateTime.Now));
    }

    public void SetAsInactive()
    {
        if (IsActive == false)
            throw new ApplicationIsNotActiveException(Id);

        IsActive = false;

        AddDomainEvent(new ChangeApplicationStatusEvent(Id, IsActive, DateTime.Now));
    }

    public override bool Equals(object obj)
    {
        return obj is Application application &&
               base.Equals(obj) &&
               EnglishName == application.EnglishName;
    }
}
