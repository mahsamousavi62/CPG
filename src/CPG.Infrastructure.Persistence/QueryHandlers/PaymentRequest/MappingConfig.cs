using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Mapster;
using System.Collections.Generic;
using System.Linq;
using CPG.Application.UseCases.PaymentRequests.Queries.Report;

namespace CPG.Infrastructure.Persistence.QueryHandlers.PaymentRequests;

public static class MappingConfig
{
    public static void RegisterMappings(Dictionary<long, string> usersCacheData)
    {
        TypeAdapterConfig<PaymentRequestReadModel, PaymentRequestReportViewModel>
            .NewConfig()
                .Map(dest => dest.CompanyId, src => src.CompanyId)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.CompanyTitle, src => src.Company.PersianName)
                .Map(dest => dest.NationalCode, src => src.NationalCode)
                .Map(dest => dest.ModificationDate, src => src.ModificationDate)
                .Map(dest => dest.PaymentCode, src => src.PaymentCode)
                .Map(dest => dest.PaymentIdentifier, src => src.PaymentIdentifier)
                .Map(dest => dest.DestinationDepositIban, src => src.DestinationDepositIban)
                .Map(dest => dest.AccountNumber, src => GetDestinationAccountNumber(src.DestinationDepositIban,
                                                                                   src.Company.CompanyDeposits))
                .Map(dest => dest.CreationDate, src => src.CreationDate)
                .Map(dest => dest.StatusEnglishName, src => src.Status.ToString())
                .Map(dest => dest.TrackerId, src => src.TrackerId)
                .Map(dest => dest.UrlExpirationDateTime, src => src.UrlExpirationDateTime)
                .Map(dest => dest.VerificationDateTime, src => src.VerificationDateTime)
                .Map(dest => dest.StatusPersianName, src => General.GetPaymentStatusName(src.Status))
                .Map(dest => dest.UserFullName, src => GetFullName(src.CreationUserId, usersCacheData));
    }

    private static string GetDestinationAccountNumber(string destinationAccountNumber, ICollection<CompanyDepositReadModel> companyDeposits)
    {
        var acc = companyDeposits.FirstOrDefault(c => c.Iban == destinationAccountNumber);
        return acc?.AccountNumber;
    }

    private static string GetFullName(long creationUserId, Dictionary<long, string> usersCacheData)
    {
        if (creationUserId == 0) return string.Empty;
        var user = usersCacheData.FirstOrDefault(c => c.Key == creationUserId);
        return user.Value;
    }
}