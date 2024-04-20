using System;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.Model;
using Mapster;

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

    public void Register(TypeAdapterConfig config)
    {
        config.ForType<TransactionReadModel, TransactionReportViewModel>();
    }
}
public static class MappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<TransactionReadModel, TransactionReportViewModel>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.CompanyId, src => src.Company.Id)
            .Map(dest => dest.CompanyPersianName, src => src.Company.PersianName)
            .Map(dest => dest.CompanyEnglishName, src => src.Company.EnglishName)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.TransactionMethodType, src => src.TransactionMethodType)
            .Map(dest => dest.TransactionMethodTypeName, src => General.GetTransactionMethodTypeName(src.TransactionMethodType))
            .Map(dest => dest.IPGTypeId, src => src.IPGTransaction.CompanyIPG.IPGTypeId)
            .Map(dest => dest.IpgTypeName, src => src.IPGTransaction.CompanyIPG.IPGType.PersianName)
            .Map(dest => dest.ProviderId, src => src.IPGTransaction.CompanyIPG.ProviderId)
            .Map(dest => dest.ProviderName, src => src.IPGTransaction.CompanyIPG.Provider.PersianName)
            .Map(dest => dest.ApplicationName, src => src.PaymentRequest.Application.PersianName)
            .Map(dest => dest.PaymentCode, src => src.PaymentRequest.PaymentCode)
            .Map(dest => dest.CompanyDepositName, src => src.DestinationDeposit.Name)
            .Map(dest => dest.CompanyDepositaccountNumber, src => src.DestinationDeposit.AccountNumber)
            .Map(dest => dest.CompanyDepositIban, src => src.DestinationDeposit.Iban)

            .Map(dest => dest.TransactionCreateDateTime, src => src.CreationDate)
            .Map(dest => dest.TransactionModificationDateTime, src => src.ModificationDate)
            .Map(dest => dest.ApplicationId, src => src.PaymentRequest.ApplicationId)
            //.Map(dest => dest.ReferenceNumber, src => src.IPGTransaction?.ReferenceNumber ??
            //                                          src.CharismaCardTransaction?.ReferenceNumber ??
            //                                          src.PaymentReceiptTransaction?.ReferenceNumber ??
            //                                          src.DirectDebitTransaction?.TrackId)
            .Map(dest => dest.TransactionStatusName, src => General.GetTransactionStatusName(src.Status))
            .Map(dest => dest.TransactionStatus, src => src.Status)
            .Map(dest => dest.TransactionStatusCode, src => src.Status.GetValue())





            ;

    }
}