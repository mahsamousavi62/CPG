namespace CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;

public class WithdrawalResponse : ResponseBase
{
    public short GrantStatus { get; set; }
    public string GrantMessage { get; set; }
    public string TrackerId { get; set; }
    public WithdrawData Data { get; set; }
}

public class WithdrawData
{
    public string Id { get; set; }
    public string AuthorizationId { get; set; }
    public int? RetryCount { get; set; }
    public long? GatewayTransactionId { get; set; }
    public string Amount { get; set; }
    public string WageAmount { get; set; }
    public string TrackId { get; set; }
    public string Status { get; set; }
    public long? PaymentNumber { get; set; }
    public string WithdrawalDate { get; set; }
    public string NotifyUrl { get; set; }
    public WithdrawPayerAccountData PayerAccount { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}

public class WithdrawPayerAccountData
{
    public string AccountNumber { get; set; }
    public string Pan { get; set; }
}
