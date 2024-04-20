using System;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class TransactionReadModel
{
    public long Id { get; set; }
    public long PaymentRquestId { get; set; }
    public long? IPGTransactionId { get; set; }
    public long? DirectDebitTransactionId { get; set; }
    public long? CharismaCardTransactionId { get; set; }
    public long? PaymentReceiptTransactionId { get; set; }
    public Enums.TransactionType TransactionMethodType { get; set; }
    public long CompanyId { get; set; }
    public long DestinationDepositId { get; set; }
    public decimal Amount { get; set; }
    public long ApplicationId { get; set; }
    public DateTime? PredictedSettlementDateTime { get; set; }
    public DateTime? CreationDate { get; set; }
    public long CreationUserId { get; set; }
    public DateTime? ModificationDate { get; set; }
    public Enums.TransactionStatus Status { get; set; }
    public PaymentRequestReadModel PaymentRequest { get; set; }
    public IPGTransactionReadModel IPGTransaction { get; set; }
    public DirectDebitTransactionReadModel DirectDebitTransaction { get; set; }
    public PaymentReceiptTransactionReadModel PaymentReceiptTransaction { get; set; }
    public CharismaCardTransactionReadModel CharismaCardTransaction { get; set; }
    public CompanyDepositReadModel DestinationDeposit { get; set; }
    public CompanyReadModel Company { get; set; }
    public ApplicationReadModel Application { get; set; }
}
