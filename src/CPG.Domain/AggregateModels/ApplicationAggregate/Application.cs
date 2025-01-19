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
    private Application(PersianName persianName, EnglishName englishName, Logo logo, Url responseApiUrl)
    {
        PersianName = persianName.Value;
        EnglishName = englishName.Value;
        Logo = logo.Value;
        ResponseApiUrl = responseApiUrl.Value;
        ApplicationIdentifiers = [];
    }

    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public string Logo { get; set; }
    public string ResponseApiUrl { get; set; }
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

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static void Update(Application application, PersianName persianName, EnglishName englishName, Url responseUrl, Logo logo,
        string[] idpClientIds, Url[] urls)
    {
        application.PersianName = persianName.Value;
        application.EnglishName = englishName.Value;
        application.ResponseApiUrl = responseUrl.Value;
        application.Logo = logo.Value;

        foreach (var newItem in idpClientIds)
        {
            if (!application.ApplicationIdentifiers.Any(p => p.IdpClientId == newItem))
                application.ApplicationIdentifiers.Add(ApplicationIdentifier.Create(newItem));
        }

        foreach (var currnetItem in application.ApplicationIdentifiers.ToList())
        {
            if (!idpClientIds.Any(p => p == currnetItem.IdpClientId))
                application.ApplicationIdentifiers.Remove(currnetItem);
        }

        if (urls != null)
        {
            foreach (var newItem in urls)
            {
                if (!application.ApplicationCallbackUrls.Any(p => p.CallbackUrl == newItem))
                    application.ApplicationCallbackUrls.Add(ApplicationCallbackUrl.Create(newItem));
            }

            foreach (var currnetItem in application.ApplicationCallbackUrls.ToList())
            {
                if (!urls.Any(p => p == currnetItem.CallbackUrl))
                    application.ApplicationCallbackUrls.Remove(currnetItem);
            }
        }
    }

    public static void Validate(Application application)
    {
        if (application == null)
            throw new ApplicationNotFoundException();
        if (!application.IsActive)
            throw new ApplicationIsNotActiveException(application.Id);
    }
}
