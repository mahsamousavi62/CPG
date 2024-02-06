using System;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class StoreRequest : RequestBase
{
    public string ProviderData { get; set; }
    public string BankCode { get; set; }
    public string MobileNumber { get; set; }
    public int Count { get; set; } = 1000;
    public decimal Limit { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string FullName { get; set; }
    public string NationalCode { get; set; }
    public string AccessToken { get; set; }
}
