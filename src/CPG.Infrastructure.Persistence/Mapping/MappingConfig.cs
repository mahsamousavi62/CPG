
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
            .Map(dest => dest.TransactionStatusName, src => General.GetTransactionStatusName(src.Status))
            .Map(dest => dest.TransactionStatus, src => src.Status)
            .Map(dest => dest.TransactionStatusCode, src => src.Status.GetValue());
    }
}



