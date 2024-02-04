using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.DbContexts;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using System.Linq;
using CPG.Infrastructure.Persistence.GraphQL.Types.Bank;
using CPG.Infrastructure.Persistence.GraphQL.Types.Company;
using Microsoft.EntityFrameworkCore;
using CPG.Infrastructure.Persistence.GraphQL.Types.Provider;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class ReadModelQueries
{
    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<BankFilterType>]
    [UseSorting<BankSortType>]
    public IQueryable<BankReadModel> GetBanks([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider)
    {
        var banks = dbContext.BankReadModels;

        var data = banks.Select(x => new BankReadModel
        {
            Id = x.Id,
            Name = x.Name,
            IbanPrefix = x.IbanPrefix,
            LogoAddress = !string.IsNullOrEmpty(x.LogoAddress) ? minioProvider.PresignedGetObject(x.LogoAddress).GetAwaiter().GetResult() : "",
            IsActive = x.IsActive,
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate
        });
                
        return data;
    }

    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<CompanyFilterType>]
    [UseSorting<CompanySortType>]
    public IQueryable<CompanyReadModel> GetCompanies([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider)
    {
        var companies = dbContext.CompanyReadModels.Include(m => m.PaymentMethods);

        var companyViewModels = companies.Select(company => new CompanyReadModel
        {
            Id = company.Id,
            PersianName = company.PersianName,
            EnglishName = company.EnglishName,
            Logo = minioProvider.PresignedGetObject(company.Logo).GetAwaiter().GetResult(),
            NationalCodeMatchingRequied = company.NationalCodeMatchingRequied,
            SiteAddress = company.SiteAddress,
            IpgRedirectionMethodType = company.IpgRedirectionMethodType,
            CreationDate = company.CreationDate,
            ModificationDate = company.ModificationDate,
            IsActive = company.IsActive,
            PaymentMethods = company.PaymentMethods.Select(p => new CompanyPaymentMethodsReadModel
            {
                Id = p.Id,
                MethodType = p.MethodType
            }
            ),
        });

        return companyViewModels;
    }

    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<CompanyFilterType>]
    [UseSorting<CompanySortType>]
    public IQueryable<ApplicationReadModel> GetApplications([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider)
    {
        var apps = dbContext.ApplicationReadModels.Include(x => x.ApplicationIdentifiers).Include(x => x.ApplicationCallbackUrls);

        var data = apps.Select(app => new ApplicationReadModel
        {
            Id = app.Id,
            PersianName = app.PersianName,
            EnglishName = app.EnglishName,
            Logo = minioProvider.PresignedGetObject(app.Logo).GetAwaiter().GetResult(),
            CreationDate = app.CreationDate,
            ModificationDate = app.ModificationDate,
            IsActive = app.IsActive,
            ResponseApiUrl = app.ResponseApiUrl,
            ApplicationIdentifiers = app.ApplicationIdentifiers != null ? app.ApplicationIdentifiers.Select(t => new ApplicationIdentifierReadModel
            {
                Id = t.Id,
                ApplicationId = t.ApplicationId,
                IdpClientId = t.IdpClientId
            }) : null,
            ApplicationCallbackUrls = app.ApplicationCallbackUrls != null ? app.ApplicationCallbackUrls.Select(t => new ApplicationCallbackUrlReadModel
            {
                Id = t.Id,
                ApplicationId = t.ApplicationId,
                CallbackUrl = t.CallbackUrl,
            }) : null,
        });

        return data;
    }

    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<ProviderFilterType>]
    [UseSorting<ProviderSortType>]
    public IQueryable<ProviderReadModel> GetProviders([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider)
    {

        var providers = dbContext.ProviderReadModels.Include(p => p.PaymentMethods);

        var data = providers.Select(x => new ProviderReadModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = minioProvider.PresignedGetObject(x.Logo).GetAwaiter().GetResult(),
            ProviderData = x.ProviderData,
            ProviderType = x.ProviderType,
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate,
            PaymentMethods = x.PaymentMethods != null ? x.PaymentMethods.Select(p => new ProviderPaymentMethodReadModel
            {
                Id = p.Id,
                MethodType = p.MethodType,
                ProviderId = p.ProviderId,
            }) : null,
        });

        return data;
    }
}