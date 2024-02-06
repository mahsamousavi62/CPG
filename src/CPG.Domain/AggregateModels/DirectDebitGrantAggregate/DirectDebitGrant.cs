using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate;

public class DirectDebitGrant : AuditableEntity<long>, IAggregateRoot
{
    internal long _userId;
    internal int _bankId;
    internal string _accountNumber;
    internal string _phoneNumber;
    internal int _successTransactionCountLimitPerMonth;
    internal decimal _amountLimitPerTransaction;
    internal string _trackId;
    internal DateTime _expirationDate;
    internal DateTime? _revokeDateTime;
    internal long _providerId;
    internal string _grantToken;
    internal string _authorizationId;
    internal DirectDebitGrantStatus _status;

    public long UserId => _userId;
    public int BankId => _bankId;
    public string AccountNumber => _accountNumber;
    public string PhoneNumber => _phoneNumber;
    public int SuccessTransactionCountLimitPerMonth => _successTransactionCountLimitPerMonth;
    public decimal AmountLimitPerTransaction => _amountLimitPerTransaction;
    public string TrackId => _trackId;
    public DateTime ExpirationDate => _expirationDate;
    public DateTime? RevokeDateTime => _revokeDateTime;
    public long ProviderId => _providerId;
    public string GrantToken => _grantToken;
    public string AuthorizationId => _authorizationId;
    public DirectDebitGrantStatus Status => _status;
    public Provider Provider { get; set; }

    public DirectDebitGrant()
    {
        
    }

    public DirectDebitGrant(long userId, int bankId, string accountNumber, string phoneNumber, int successTransactionCountLimitPerMonth,
        decimal amountLimitPerTransaction, string trackId, DateTime expirationDate, DateTime? revokeDateTime, long providerId, string grantToken,
        string authorizationId, DirectDebitGrantStatus status)
    {
        _userId = userId;
        _bankId = bankId;
        _accountNumber = accountNumber;
        _phoneNumber = phoneNumber;
        _successTransactionCountLimitPerMonth = successTransactionCountLimitPerMonth;
        _amountLimitPerTransaction = amountLimitPerTransaction;
        _trackId = trackId;
        _expirationDate = expirationDate;
        _revokeDateTime = revokeDateTime;
        _providerId = providerId;
        _grantToken = grantToken;
        _authorizationId = authorizationId;
        _status = status;
    }

    public static DirectDebitGrant Create(long userId, int bankId, string accountNumber, string phoneNumber, int successTransactionCountLimitPerMonth,
        decimal amountLimitPerTransaction, string trackId, DateTime expirationDate, DateTime? revokeDateTime, long providerId, string grantToken,
        string authorizationId, DirectDebitGrantStatus status)
    {
        var directDebitGrant = new DirectDebitGrant(userId, bankId, accountNumber, phoneNumber, successTransactionCountLimitPerMonth,
            amountLimitPerTransaction, trackId, expirationDate, revokeDateTime, providerId, grantToken, authorizationId, status);
        return directDebitGrant;
    }

    public void Update(long userId, int bankId, string accountNumber, string phoneNumber, int successTransactionCountLimitPerMonth,
        decimal amountLimitPerTransaction, string trackId, DateTime expirationDate, DateTime? revokeDateTime, long providerId, string grantToken,
        string authorizationId, DirectDebitGrantStatus status)
    {
        _userId = userId;
        _bankId = bankId;
        _accountNumber = accountNumber;
        _phoneNumber = phoneNumber;
        _successTransactionCountLimitPerMonth = successTransactionCountLimitPerMonth;
        _amountLimitPerTransaction = amountLimitPerTransaction;
        _trackId = trackId;
        _expirationDate = expirationDate;
        _revokeDateTime = revokeDateTime;
        _providerId = providerId;
        _grantToken = grantToken;
        _authorizationId = authorizationId;
        _status = status;
    }
}