using System;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class ProviderReadModel
{
    public long Id { get; set; }
    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public ProviderType ProviderType { get; set; }
    public string Logo { get; set; }
    public string ProviderData { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public IEnumerable<ProviderPaymentMethodReadModel> PaymentMethods { get; set; }
    public IEnumerable<BankDirectDebitSettingReadModel> BankDirectDebitSettings { get;}
}
