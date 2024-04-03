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
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Infrastructure.Persistence.GraphQL.Model;
using System;
using System.Collections.Generic;
using CPG.Infrastructure.Persistence.GraphQL.Types.Transaction;
using Mapster;
using CPG.Domain.SharedKernel;
using HotChocolate.Authorization;


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
            Logo = !string.IsNullOrEmpty(x.Logo) ? minioProvider.PresignedGetObject(x.Logo).GetAwaiter().GetResult() : "",
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
                 BankLogo = minioProvider.PresignedGetObject(company.Bank.Logo).GetAwaiter().GetResult(),
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
            BankLogo = minioProvider.PresignedGetObject(company.Bank.Logo).GetAwaiter().GetResult(),
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
            CreationDate = entity.CreationDate,
            ModificationDate = entity.ModificationDate,
            IsActive = entity.IsActive,

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
            CreationDate = entity.CreationDate,
            ModificationDate = entity.ModificationDate,
            IsActive = entity.IsActive,

        });
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

    // [Authorize(Roles = new[] { "Guest", "Admin" })]
    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<TransactionFilerType>]
    [UseSorting<TransactionSortType>]
    public async Task<IEnumerable<TransactionReportViewModel>> GetTransactions([Service] ReadDbContext dbContext,
        [Service] IMinioProvider minioProvider, int? pageNumber,int? pageSize)
    {
        long companyId = 1;

        if (!pageNumber.HasValue)
        {
            pageNumber = 1;
        }
        if (!pageSize.HasValue)
        {
            pageSize = 10;
        }

        var data = await dbContext.TransactionReadModels
            .Where(c => c.CompanyId == companyId).OrderByDescending(c => c.Id)
            .Select(c => new
            {
                Transaction = c,
                Company = c.Company,
                PaymentRequest = c.PaymentRequest,
                Application = c.PaymentRequest.Application,
                IPGTransaction = c.IPGTransaction,
                CompanyIPG = c.IPGTransaction != null ? c.IPGTransaction.CompanyIPG : null,
                IPGType = c.IPGTransaction.CompanyIPG.IPGType,
                Provider = c.IPGTransaction.CompanyIPG.Provider,
                CompanyDeposit = c.DestinationDeposit
            })
            .Skip((pageNumber.Value - 1) * pageSize.Value) .Take(pageSize.Value)
            .ToListAsync();
        
        var users = await dbContext.UserReadModels.ToListAsync();
        var viewModels = await Task.WhenAll(

             data.Select(async entity => new TransactionReportViewModel
             {
                 Id = entity.Transaction.Id,
                 CompanyId = entity.Company.Id,
                 CompanyPersianName = entity.Company.PersianName,
                 CompanyEnglishName = entity.Company.EnglishName,
                 Amount = entity.Transaction.Amount,
                 TransactionMethodType = entity.Transaction.TransactionMethodType,
                 TransactionMethodTypeName = GetTransactionMethodTypeName(entity.Transaction.TransactionMethodType),
                 IpgTypeName = entity.IPGType?.PersianName,
                 ProviderName = entity.Provider?.PersianName,
                 ApplicationName = entity.Application?.PersianName,
                 PaymentCode = entity.PaymentRequest?.PaymentCode,
                 CompanyDepositName = entity.CompanyDeposit.Name,
                 CompanyDepositaccountNumber = entity.CompanyDeposit.AccountNumber,
                 CompanyDepositIban = entity.CompanyDeposit.Iban,
                 CompanyLogo = !string.IsNullOrEmpty(entity.Company.Logo) ? await GetCompanyLogo(minioProvider, entity.Company.Logo) : null,
                 TransactionCreateDateTime = entity.Transaction.CreationDate,
                 TransactionModificationDateTime = entity.Transaction.ModificationDate,
                 FirstName = users.FirstOrDefault(c => c.Id == entity.Transaction.CreationUserId)?.FirstName,
                 LastName = users.FirstOrDefault(c => c.Id == entity.Transaction.CreationUserId)?.LastName,
                 NationalCode = entity.PaymentRequest.NationalCode,
                 ApplicationId = entity.Application.Id,
                 ReferenceNumber = entity.IPGTransaction?.ReferenceNumber,
                 TransactionStatus = GetTransactionStatusName(entity.Transaction.Status),
                 Status=entity.Transaction.Status
             }).ToList()
            );

        return viewModels.OrderByDescending(c => c.Id);
    }

    private string GetTransactionStatusName(Enums.TransactionStatus status)
    {
        switch (status)
        {
            case Enums.TransactionStatus.InPrgress:
                return "در حال انجام";
            case Enums.TransactionStatus.TransactionSucceeded:
                return "تراکنش موفق";
            case Enums.TransactionStatus.TransactionFailed:
                return "تراکنش ناموفق";
            default:
                return string.Empty;
        }
    }

    private async Task<string> GetCompanyLogo(IMinioProvider minioProvider, string logoPath)
    {
        try
        {
            return await minioProvider.PresignedGetObject(logoPath);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private string GetTransactionMethodTypeName(Enums.TransactionType transactionMethodType)
    {
        switch (transactionMethodType)
        {
            case Enums.TransactionType.IPG:
                return "درگاه پرداخت";
            case Enums.TransactionType.DirectDebit:
                return "برداشت مستقیم";
            case Enums.TransactionType.PaymentReceipt:
                return "فیش واریزی";
            case Enums.TransactionType.CharismaCard:
                return "کاریزما کارت";
            default:
                return string.Empty;
        }
    }
}
