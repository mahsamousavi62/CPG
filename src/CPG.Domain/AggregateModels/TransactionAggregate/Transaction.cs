
using System;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class Transaction : AuditableEntity<long>, IAggregateRoot
{
    public Transaction(long paymentRquestId, long referenceTransactionId, Enums.TransactionType transactionMethodType, long companyId,
                   long destinationDepositId, decimal amount, long applicationId, DateTime predictedSettlementDateTime)
    {
        PaymentRquestId = paymentRquestId;
        ReferenceTransactionId = referenceTransactionId;
        TransactionMethodType = transactionMethodType;
        CompanyId = companyId;
        DestinationDepositId = destinationDepositId;
        Amount = amount;
        ApplicationId = applicationId;
        PredictedSettlementDateTime = predictedSettlementDateTime;
    }

    public long PaymentRquestId { get; set; }
    public long ReferenceTransactionId { get; set; }
    public Enums.TransactionType TransactionMethodType { get; set; }
    public long CompanyId { get; set; }
    public long DestinationDepositId { get; set; }
    public decimal Amount { get; set; }
    public long ApplicationId { get; set; }
    public DateTime PredictedSettlementDateTime { get; set; }
    public short Status { get; set; }
    public PaymentRequest PaymentRequest { get; set; }
    public IPGTransaction IPGTransaction { get; set; }

    public static object Create(long id1, long id2, short transactionMethodType, int userId, long companyId, long destinationDepositId, decimal amount, long id3, short status)
    {
        throw new NotImplementedException();
    }
}
