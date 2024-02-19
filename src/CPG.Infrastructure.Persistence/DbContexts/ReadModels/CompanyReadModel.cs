using System;
using System.Collections.Generic;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class CompanyReadModel
{
    public long Id { get; set; }
    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public bool NationalCodeMatchingRequied { get; set; }
    public string SiteAddress { get; set; }
    public short IpgRedirectionMethodType { get; set; }
    public string Logo { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public IEnumerable<CompanyPaymentMethodsReadModel> PaymentMethods { get; set; }
    public ICollection<CompanyDepositReadModel> CompanyDeposits { get; set; }
    public ICollection<PaymentRequestReadModel> PaymentRequests { get; set; }
    public CompanyShaparakSettingReadModel ShaparakSetting { get; set; }
    public ICollection<UserReadModel> Users  { get; set; }

}
