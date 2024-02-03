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
using CPG.Application.UseCases.IPGTypes.ViewModels;
using System.Threading.Tasks;
using System.Threading;
using CPG.Infrastructure.Persistence.GraphQL.Types.CompanyDeposit;
using static HotChocolate.ErrorCodes;

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


    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<CompanyDepositFilterType>]
    [UseSorting<CompanayDepositSortType>]
    public IQueryable<CompanyDepositReadModel> GetCompanyDeposits([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider)
    {
        var companyDeposits = dbContext.CompanyDepositReadModels.Include(c => c.Bank).Include(c => c.Company);
        var companyDepositViewModels =
             companyDeposits.Select(company => new CompanyDepositReadModel
             {
                 Id = company.Id,
                 Name = company.Name,
                 AccountNumber = company.AccountNumber,
                 Iban = company.Iban,
                 BankId = company.BankId,
                 BankLogo = minioProvider.PresignedGetObject(company.Bank.LogoAddress).GetAwaiter().GetResult(),
                 BankName = company.Bank.Name,
                 CompanyId = company.CompanyId,
                 CompanyName = company.Company.PersianName,
                 CreationDate = company.CreationDate,
                 IsActive = company.IsActive,
                 ModificationDate = company.ModificationDate,
             });
        return companyDepositViewModels;
    }



    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<CompanyDepositFilterType>]
    [UseSorting<CompanayDepositSortType>]
    public IQueryable<CompanyDepositReadModel> GetCompanyDepositsByCompanyId([Service] ReadDbContext dbContext,
        [Service] IMinioProvider minioProvider, long companyId)
    {
        var companyDeposits = dbContext.CompanyDepositReadModels.Include(c => c.Bank).Include(c => c.Company).Where(c => c.CompanyId == companyId);
        var companyDepositViewModels = companyDeposits.Select(company => new CompanyDepositReadModel
        {
            Id = company.Id,
            Name = company.Name,
            AccountNumber = company.AccountNumber,
            Iban = company.Iban,
            BankId = company.BankId,
            BankLogo = minioProvider.PresignedGetObject(company.Bank.LogoAddress).GetAwaiter().GetResult(),
            BankName = company.Bank.Name,
            CompanyId = company.CompanyId,
            CompanyName = company.Company.PersianName,
            CreationDate = company.CreationDate,
            IsActive = company.IsActive,
            ModificationDate = company.ModificationDate,
        });
        return companyDepositViewModels;
    }


    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<IPGTypeFilterType>]
    [UseSorting<IPGTypeSortType>]
    public IQueryable<IPGTypeReadModel> GetIPGs([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider)
    {
        var ipgTypes = dbContext.IPGTypeReadModels;

        var viewModels = ipgTypes.Select(x => new IPGTypeReadModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = minioProvider.PresignedGetObject(x.Logo).GetAwaiter().GetResult(),
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate,
        });
        return ipgTypes;
    }


    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<CompanyIPGFilterType>]
    [UseSorting<CompanayIPGSortType>]
    public IQueryable<CompanyIPGReadModel> GetCompanyIPGsbyCompanyId([Service] ReadDbContext dbContext,
        [Service] IMinioProvider minioProvider, long companyId)
    {
        var data = dbContext.CompanyIPGReadModels
           .Include(t => t.CompanyIPGDeposits)
           .ThenInclude(t => t.CompanyDeposit)
           .Include(t => t.IPGType)
           .Include(t => t.Provider)
        .Where(t => t.CompanyId == companyId);

        var viewModels = data.Select(entity => new CompanyIPGReadModel
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            ProviderData = entity.ProviderData,
            IPGTypeId = entity.IPGTypeId,
            ProviderId = entity.ProviderId,
            CompanyIPGDeposits = entity.CompanyIPGDeposits.
            Select(t => new CompanyIPGDepositReadModel { Id = t.Id, AccountNumber = t.CompanyDeposit.AccountNumber, Name = t.CompanyDeposit.Name }).ToList(),
            CreationDate = entity.CreationDate,
            ModificationDate = entity.ModificationDate,
            IsActive = entity.IsActive,
            DefaultDeposit = entity.CompanyIPGDeposits.Where(t => t.IsDefault == true).
            Select(t => new CompanyIPGDepositReadModel { Id = t.Id, AccountNumber = t.CompanyDeposit.AccountNumber, Name = t.CompanyDeposit.Name }).FirstOrDefault(),
            IPGTypeLogo = minioProvider.PresignedGetObject(entity.IPGType.Logo).GetAwaiter().GetResult(),
            IPGTypeName = entity.IPGType.PersianName,
            ProviderName = entity.Provider.PersianName,
        });
        return viewModels;
    }


    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<CompanyIPGFilterType>]
    [UseSorting<CompanayIPGSortType>]
    public IQueryable<CompanyIPGReadModel> GetCompanyIPGs([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider)
    {
        var data = dbContext.CompanyIPGReadModels
           .Include(t => t.CompanyIPGDeposits)
           .ThenInclude(t => t.CompanyDeposit)
           .Include(t => t.IPGType)
           .Include(t => t.Provider);

        var viewModels = data.Select(entity => new CompanyIPGReadModel
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            ProviderData = entity.ProviderData,
            IPGTypeId = entity.IPGTypeId,
            ProviderId = entity.ProviderId,
            CompanyIPGDeposits = entity.CompanyIPGDeposits.
            Select(t => new CompanyIPGDepositReadModel { Id = t.Id, AccountNumber = t.CompanyDeposit.AccountNumber, Name = t.CompanyDeposit.Name }).ToList(),
            CreationDate = entity.CreationDate,
            ModificationDate = entity.ModificationDate,
            IsActive = entity.IsActive,
            DefaultDeposit = entity.CompanyIPGDeposits.Where(t => t.IsDefault == true).
            Select(t => new CompanyIPGDepositReadModel { Id = t.Id, AccountNumber = t.CompanyDeposit.AccountNumber, Name = t.CompanyDeposit.Name }).FirstOrDefault(),
            IPGTypeLogo = minioProvider.PresignedGetObject(entity.IPGType.Logo).GetAwaiter().GetResult(),
            IPGTypeName = entity.IPGType.PersianName,
            ProviderName = entity.Provider.PersianName,
        }); ; ;
        return viewModels;
    }


    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<UserFilterType>]
    [UseSorting<UserSortType>]
    public IQueryable<UserReadModel> GetCompanyUsers([Service] ReadDbContext dbContext)
    {
        var users = dbContext.UserReadModels.
          Where(u => u.KYCStatus == 1 && u.IsActive && u.CompanyId == null);
        return users;
    }


    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<UserFilterType>]
    [UseSorting<UserSortType>]
    public IQueryable<UserReadModel> GetUsers([Service] ReadDbContext dbContext)
    {
        var data = dbContext.UserReadModels.Include(c => c.Company).Include(c => c.UserRoles);

        var viewModels = data.Select(entity => new UserReadModel
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            CompanyName = entity.Company.PersianName,
            FirstName = entity.FirstName,
            IDPId = entity.IDPId,
            IsActive = entity.IsActive,
            IsLegal = entity.IsLegal,
            LastName = entity.LastName,
            LastUpdateFromIDP = entity.LastUpdateFromIDP,
            CreationDate = entity.CreationDate,
            ModificationDate = entity.ModificationDate,
            NationalCode = entity.NationalCode,
            PhoneNumber = entity.PhoneNumber,
            UserRoles = entity.UserRoles,

        });
        return viewModels;
    }

}
