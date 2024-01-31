using CPG.Domain.AggregateModels.ApplicationAggregate.Events;
using CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
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

    public static void Update(Application application, PersianName persianName, EnglishName englishName, Url responseUrl, Logo logo, 
        string[] idpClientIds, Url[] urls)
    {
        application.PersianName = persianName.Value;
        application.EnglishName = englishName.Value;
        application.ResponseApiUrl=responseUrl.Value;
        application.Logo=logo.Value;
        application.ApplicationIdentifiers.Clear();
        application.ApplicationCallbackUrls.Clear();
        var applicationIdentifiers = ApplicationIdentifier.Create(idpClientIds);
        application.ApplicationIdentifiers.AddRange(applicationIdentifiers);

        if (urls?.Length > 0 is true)
        {
            var applicationCallbackUrls = ApplicationCallbackUrl.Create(urls);
            application.ApplicationCallbackUrls.AddRange(applicationCallbackUrls);
        }

    }


    private void test (long[] applicationIdentifiers, Application entity)
    {
        if (applicationIdentifiers == null)
        {
            entity.ApplicationIdentifiers = new List<ApplicationIdentifier>();
            return;
        }

        var selectedCoursesHS = new HashSet<long>(applicationIdentifiers);
        var instructorCourses = new HashSet<long>(entity.ApplicationIdentifiers.Select(c => c.Id));
        entity.ApplicationIdentifiers.Clear();

    }
}
