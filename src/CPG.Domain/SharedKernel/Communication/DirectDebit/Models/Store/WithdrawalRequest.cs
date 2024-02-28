using System;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class WithdrawalRequest : RequestBase
{
    public string ProviderData { get; set; }
    public string AccessToken { get; set; }
    public string GrantAuthorizationId { get; set; }
    public decimal Amount { get; set; }
    public DateTime? WithdrawalDate { get; set; }
    public bool IsInstant { get; set; }
    public string Description { get; set; }
    public short MaxRetryCount { get; set; }
}
