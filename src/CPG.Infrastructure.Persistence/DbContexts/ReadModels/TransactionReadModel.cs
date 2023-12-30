
using System;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class TransactionReadModel 
{
    public long Id { get; set; }
    public long PaymentRquestId { get; set; }
    public long IPGTransactionId { get; set; }
    public Enums.TransactionType TransactionMethodType { get; set; }
    public long CompanyId { get; set; }
    public long DestinationDepositId { get; set; }
    public decimal Amount { get; set; }
    public long ApplicationId { get; set; }
    public DateTime PredictedSettlementDateTime { get; set; }
    public Enums.TransactionStatus Status { get; set; }
    public PaymentRequestReadModel PaymentRequest { get; set; }
    public IPGTransactionReadModel IPGTransaction { get; set; }

}
