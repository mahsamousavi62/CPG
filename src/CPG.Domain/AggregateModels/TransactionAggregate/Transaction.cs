
using System;
using CPG.Domain.SeedWork;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class Transaction : AuditableEntity<long>, IAggregateRoot
{
    
    public long PaymentRquestId { get; set; }

    public Transaction(long paymentRquestId, long iPGTransactionId, short transactionMethodType, long companyId,
                       long destinationDepositId, decimal amount, long applicationId, DateTime predictedSettlementDateTime)
    {
        PaymentRquestId = paymentRquestId;
        IPGTransactionId = iPGTransactionId;
        TransactionMethodType = transactionMethodType;
        CompanyId = companyId;
        DestinationDepositId = destinationDepositId;
        Amount = amount;
        ApplicationId = applicationId;
        PredictedSettlementDateTime = predictedSettlementDateTime;
    }

    public long IPGTransactionId { get; set; }
    public short TransactionMethodType { get; set; }
    public long CompanyId { get; set; }
    public long DestinationDepositId { get; set; }
    public decimal Amount { get; set; }
    public long ApplicationId { get; set; }
    public DateTime PredictedSettlementDateTime { get; set; }
    public short Status { get; set; }
    public PaymentRequest PaymentRequest { get; set; }
    public IPGTransaction IPGTransaction { get; set; }

}
