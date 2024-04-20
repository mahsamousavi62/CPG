
namespace CPG.Infrastructure.Persistence.Mapping;

using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.GraphQL.Model;
using Mapster;

public static class MappingConfig
{
    public static void Configure()
    {
       
            //TypeAdapterConfig<TransactionReadModel, TransactionReportViewModel>.NewConfig()
            //.Map(dest => dest.TransactionCreateDateTime, src => src.CreationDate)
            //.Map(dest => dest.TransactionModificationDateTime, src => src.ModificationDate)
            //.Map(dest => dest.TransactionStatus, src => src.Status)
            //.Map(dest => dest.TransactionStatusCode, src => src.Status.GetValue())
            //.Map(dest => dest.TransactionMethodType, src => src.TransactionMethodType)
            //.Map(dest => dest.TransactionMethodTypeName, src => General.GetTransactionMethodTypeName(src.TransactionMethodType))
            //.Map(dest => dest.CompanyId, src => src.Company.Id)
            //.Map(dest => dest.CompanyPersianName, src => src.Company.PersianName)
            //.Map(dest => dest.CompanyEnglishName, src => src.Company.EnglishName)
            //.Map(dest => dest.Amount, src => src.Amount)
            //.Map(dest => dest.ApplicationId, src => src.ApplicationId)
            //.Map(dest => dest.ApplicationName, src => src.Application.PersianName)
            //.Map(dest => dest.ReferenceNumber, src => src.IPGTransaction?.ReferenceNumber ??
            //                                       src.CharismaCardTransaction?.ReferenceNumber ??
            //                                       src.PaymentReceiptTransaction?.ReferenceNumber ??
            //                                       src.DirectDebitTransaction?.TrackId)
            //.Map(dest => dest.TransactionStatusName, src => General.GetTransactionStatusName(src.Status))
            //.Map(dest => dest.IPGTypeId, src => src.IPGTransaction.CompanyIPG.IPGType?.Id ?? 0)
            //.Map(dest => dest.IpgTypeName, src => src.IPGType?.PersianName)
            //.Map(dest => dest.ProviderId, src => src.Provider?.Id ?? 0)
            //.Map(dest => dest.ProviderName, src => src.Provider?.PersianName)
            //.Map(dest => dest.PaymentCode, src => src.PaymentRequest?.PaymentCode)
            //.Map(dest => dest.CompanyDepositName, src => src.CompanyDeposit?.Name)
            //.Map(dest => dest.CompanyDepositaccountNumber, src => src.CompanyDeposit?.AccountNumber)
            //.Map(dest => dest.CompanyDepositIban, src => src.CompanyDeposit?.Iban)
            //.Map(dest => dest.FirstName, src => UserscacheData.FirstOrDefault(c => c.Id == src.Transaction.CreationUserId)?.FirstName)
            //.Map(dest => dest.LastName, src => UserscacheData.FirstOrDefault(c => c.Id == src.Transaction.CreationUserId)?.LastName)
            //.Map(dest => dest.NationalCode, src => UserscacheData.FirstOrDefault(c => c.Id == src.Transaction.CreationUserId)?.NationalCode)
            //.Map(dest => dest.CompanyLogo, async (src, dest) => !string.IsNullOrEmpty(src.Company.Logo) ? await General.GetLogo(minioProvider, src.Company.Logo) : null);

        ;
    }
}


