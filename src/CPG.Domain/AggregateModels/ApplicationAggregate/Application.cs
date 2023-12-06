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
    public Application(PersianName persianName, EnglishName englishName, Logo logo, string responseApiUrl)
    {
        _persianName = persianName.Value;
        _englishName = englishName.Value;
        _logo = logo.Value;
        _responseApiUrl = responseApiUrl;
        ApplicationIdentifiers = new List<ApplicationIdentifier>();
    }

    private string _persianName;
    private string _englishName;
    private string _logo;
    private string _responseApiUrl;
    public string PersianName => _persianName;
    public string EnglishName => _englishName;
    public string Logo => _logo;
    public string ResponseApiUrl => _responseApiUrl;
    public List<ApplicationIdentifier> ApplicationIdentifiers { get; set; } = [];
    public List<ApplicationCallbackUrl> ApplicationCallbackUrls { get; set; } = [];

    public static Application Create(PersianName persianName, EnglishName englishName, string responseApiUrl, Logo logo, string[] details, string[] callbackUrls)
    {
        var application = new Application(persianName, englishName, logo, responseApiUrl);

        var applicationIdentifiers = ApplicationIdentifier.Create(details);
        application.ApplicationIdentifiers.AddRange(applicationIdentifiers);

        if (callbackUrls?.Any() is true)
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
}
