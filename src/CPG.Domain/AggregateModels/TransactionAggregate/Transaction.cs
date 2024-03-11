using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class Transaction : AuditableEntity<long>, IAggregateRoot
{
    public Transaction(long paymentRquestId, Enums.TransactionType transactionMethodType, long companyId,
                   long destinationDepositId, decimal amount, long applicationId, Enums.TransactionStatus status)
    {
        PaymentRquestId = paymentRquestId;
        TransactionMethodType = transactionMethodType;
        CompanyId = companyId;
        DestinationDepositId = destinationDepositId;
        Amount = amount;
        ApplicationId = applicationId;
        Status = status;
        IsActive = true;
    }

    public long PaymentRquestId { get; set; }

    public long? IPGTransactionId { get; set; }

    public long? DirectDebitTransactionId { get; set; }

    public long? PaymentReceiptTransactionId { get; set; }

    public Enums.TransactionType TransactionMethodType { get; set; }

    public long CompanyId { get; set; }

    public long DestinationDepositId { get; set; }

    public decimal Amount { get; set; }

    public long ApplicationId { get; set; }

    public DateTime? PredictedSettlementDateTime { get; set; }

    public Enums.TransactionStatus Status { get; set; }

    public PaymentRequest PaymentRequest { get; set; }

    public IPGTransaction IPGTransaction { get; set; }

    public DirectDebitTransaction DirectDebitTransaction { get; set; }

    public PaymentReceiptTransaction PaymentReceiptTransaction { get; set; }

    public CompanyDeposit DestinationDeposit { get; set; }

    public static Transaction Create(CreateTransactionModel model)
    {
        Transaction transaction = new(model.PaymentRequest.Id, model.TransactionMethodType, model.PaymentRequest.Company.Id,
                                      model.DestinationDepositId, model.PaymentRequest.Amount, model.PaymentRequest.Application.Id,
                                      model.Status);

        switch (model.TransactionMethodType)
        {
            case Enums.TransactionType.IPG:
                {
                    var ipgTransaction = IPGTransaction.Create(model.IPGTransactionModel.TrackId, Enums.IPGTransactionStatus.WaitingForPspResponse,
                        model.IPGTransactionModel.CompanyIPG.Id, model.IPGTransactionModel.Token, model.IPGTransactionModel.IpgVerificationTimeLimit);

                    transaction.IPGTransaction = ipgTransaction;
                    break;
                }
            case Enums.TransactionType.DirectDebit:
                {
                    var ddTransaction = DirectDebitTransaction.Create(model.DDTransactionModel.GrantId, model.DDTransactionModel.Status,
                        model.DDTransactionModel.TrackId, model.DDTransactionModel.ProviderTrackId, model.DDTransactionModel.ProviderData);
                    transaction.DirectDebitTransaction = ddTransaction;
                    break;
                }
            case Enums.TransactionType.PaymentReceipt:
                {
                    var paymentReceipt = PaymentReceiptTransaction.Create(model.PaymentReceiptModel.SourceIban, model.PaymentReceiptModel.ReferenceNumber,
                        model.PaymentReceiptModel.ReceiptDateTime, model.PaymentReceiptModel.Description, model.PaymentReceiptModel.ReceiptImage,
                        model.PaymentReceiptModel.Status);
                    transaction.PaymentReceiptTransaction = paymentReceipt;
                    break;
                }
            default:
                break;
        }

        return transaction;
    }
}
