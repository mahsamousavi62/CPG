
using System;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System.Net.NetworkInformation;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.SharedKernel.Communication.Idp.Models.UserProfile;
using System.Transactions;
using CPG.Application.UseCases.CompanyDeposits;

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
    public long IPGTransactionId { get; set; }
    public Enums.TransactionType TransactionMethodType { get; set; }
    public long CompanyId { get; set; }
    public long DestinationDepositId { get; set; }
    public decimal Amount { get; set; }
    public long ApplicationId { get; set; }
    public DateTime PredictedSettlementDateTime { get; set; }
    public Enums.TransactionStatus Status { get; set; }
    public PaymentRequest PaymentRequest { get; set; }
    public IPGTransaction IPGTransaction { get; set; }
    public CompanyDeposit DestinationDeposit { get; set; }

    public static Transaction Create(CreateTransactionModel model)
    {
        Transaction transaction = new Transaction(model.PaymentRequest.Id,
                                                  model.TransactionMethodType, model.PaymentRequest.Company.Id,
                                                  model.DestinationDepositId, model.PaymentRequest.Amount,
                                                  model.PaymentRequest.Application.Id, Enums.TransactionStatus.InPrgress);

        var ipgTransaction = IPGTransaction.Create(model.TrackId, Enums.IPGTransactionStatus.WaitingForPspResponse,
                                                   model.CompanyIPG.Id, model.Token, model.CompanyIPG.VerificationTimeLimit);

        transaction.IPGTransaction = ipgTransaction;

        return transaction;
    }
}
