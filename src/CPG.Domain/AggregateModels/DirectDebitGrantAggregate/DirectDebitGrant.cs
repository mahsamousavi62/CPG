using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate;

public class DirectDebitGrant : AuditableEntity<long>, IAggregateRoot
{
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
    public DirectDebitGrantStatus Status { get; set; }
    public Provider Provider { get; set; }

    public DirectDebitGrant()
    {
        
    }

    public DirectDebitGrant(long userId, int bankId, string accountNumber, string phoneNumber, int successTransactionCountLimitPerMonth,
        decimal amountLimitPerTransaction, string trackId, DateTime expirationDate, DateTime? revokeDateTime, long providerId, string grantToken,
        string authorizationId, DirectDebitGrantStatus status)
    {
        UserId = userId;
        BankId = bankId;
        AccountNumber = accountNumber;
        PhoneNumber = phoneNumber;
        SuccessTransactionCountLimitPerMonth = successTransactionCountLimitPerMonth;
        AmountLimitPerTransaction = amountLimitPerTransaction;
        TrackId = trackId;
        ExpirationDate = expirationDate;
        RevokeDateTime = revokeDateTime;
        ProviderId = providerId;
        GrantToken = grantToken;
        AuthorizationId = authorizationId;
        Status = status;
    }

    public static DirectDebitGrant Create(long userId, int bankId, string accountNumber, string phoneNumber, int successTransactionCountLimitPerMonth,
        decimal amountLimitPerTransaction, string trackId, DateTime expirationDate, DateTime? revokeDateTime, long providerId, string grantToken,
        string authorizationId, DirectDebitGrantStatus status)
    {
        var directDebitGrant = new DirectDebitGrant(userId, bankId, accountNumber, phoneNumber, successTransactionCountLimitPerMonth,
            amountLimitPerTransaction, trackId, expirationDate, revokeDateTime, providerId, grantToken, authorizationId, status);
        directDebitGrant.IsActive = true;
        return directDebitGrant;
    }
}