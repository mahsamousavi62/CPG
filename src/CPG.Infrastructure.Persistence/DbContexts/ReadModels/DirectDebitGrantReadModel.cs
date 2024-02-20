using System;
using System.Collections.Generic;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class DirectDebitGrantReadModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public int BankId { get; set; }
    public string AccountNumber { get; set; }
    public string PhoneNumber { get; set; }
    public int SuccessTransactionCountLimitPerMonth { get; set; }
    public decimal AmountLimitPerTransaction { get; set; }
    public string TrackId { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime? RevokeDateTime { get; set; }
    public long ProviderId { get; set; }
    public string GrantToken { get; set; }
    public string AuthorizationId { get; set; }
    public short Status { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public short DurationPerMonth { get; set; }
    public ProviderReadModel Provider { get; set; }
    public BankReadModel Bank { get; set; }
    public List<DirectDebitTransactionReadModel> DirectDebitTransactions { get; set; }
}