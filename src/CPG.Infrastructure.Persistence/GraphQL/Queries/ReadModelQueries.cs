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
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Infrastructure.Persistence.Redis;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Infrastructure.Persistence.GraphQL.Types.IPGTransaction;
using System.Security.Cryptography.X509Certificates;
using CPG.Infrastructure.Persistence.GraphQL.Types.PaymentReceiptTransaction;
using CPG.Infrastructure.Persistence.GraphQL.Types.CharismaCard;
using CPG.Application.UseCases.Companies.Exceptions;


namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class ReadModelQueries
{
    #region Gets
    [Authorize(Policy = AuthPolicies.Roles.Admin)]
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
    #endregion

    #region [ IPGTranactionReport ]

    [Authorize(Policy = AuthPolicies.Roles.AdminOrCompanyUser)]
    [UseFiltering<IPGTransactionFilterType>]
    [UseSorting<IPGTransactionSortType>]
    public async Task<ReportViewModel<IpgTransactionReportViewModel>> GetIPGTransactions
   ([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider,
   [Service] IHttpContextAccessor httpContext, int? pageNumber, int? pageSize)
    {
        pageNumber ??= 1;
        pageSize ??= 10;

        IQueryable<IPGTransactionReadModel> query = dbContext.IPGTransactionReadModels.Include(c => c.Transaction);

        var roleClaim = httpContext.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Role &&
        c.Value == UserRoleType.SuperAdmin.GetValue());
        if (roleClaim == null)
        {
            var companyIdClaim = httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (companyIdClaim == null ||
            !long.TryParse(companyIdClaim.Value, out long companyId) || companyId == 0)
            {
                throw new CompanyNotFoundException(0);
            }
            else
            {
                query = query.Where(c => c.Transaction.CompanyId == companyId);
            }
        }
        int totalCount = query.Count();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var data = await query.OrderByDescending(c => c.Id)
            .Select(c => new
            {
                IPGTransaction = c,
                Company = c.Transaction.Company,
                PaymentRequest = c.Transaction.PaymentRequest,
                Transaction = c.Transaction,
                IPGType = c.CompanyIPG.IPGType,
            })
            .Skip((pageNumber.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync();

        var viewModels = await Task.WhenAll(

             data.Select(async entity => new IpgTransactionReportViewModel
             {
                 IPGToken = entity.IPGTransaction.IPGToken,
                 Id = entity.Transaction.Id,
                 CompanyId = entity.Company.Id,
                 CompanyPersianName = entity.Company.PersianName,
                 CompanyEnglishName = entity.Company.EnglishName,
                 Amount = entity.Transaction.Amount,
                 IPGTypeId = entity.IPGType.Id,
                 IpgTypePersianName = entity.IPGType?.PersianName,
                 IpgTypeLogo = !string.IsNullOrEmpty(entity.IPGType.Logo) ? await General.GetLogo(minioProvider, entity.IPGType.Logo) : null,
                 PaymentCode = entity.PaymentRequest?.PaymentCode,
                 CompanyLogo = !string.IsNullOrEmpty(entity.Company.Logo) ? await General.GetLogo(minioProvider, entity.Company.Logo) : null,
                 CreationDate = entity.IPGTransaction.CreationDate,
                 VerificationDateTime = entity.IPGTransaction.VerificationDateTime,
                 PredicateExpirationDateTime = entity.IPGTransaction.PredicateExpirationDateTime,
                 PredictedSettlementDateTime = entity.Transaction.PredictedSettlementDateTime,
                 ReferenceNumber = entity.IPGTransaction?.ReferenceNumber,
                 TransactionStatusName = General.GetIPGTransactionStatusName(entity.IPGTransaction.Status),
                 TransactionStatus = entity.IPGTransaction.Status,
                 TransactionStatusCode = entity.IPGTransaction.Status.GetValue(),
                 TrackerId = entity.IPGTransaction.TrackId,
                 ProviderTrackerId = entity.IPGTransaction.ProviderTrackerId,

             }).ToList()
            );

        return new ReportViewModel<IpgTransactionReportViewModel>
        {
            TotalCount = totalCount,
            CurrentPage = pageNumber.Value,
            TotalPages = totalPages,
            HasNextPage = pageNumber < totalPages,
            HasPreviousPage = pageNumber > 1,
            Models = viewModels
        }; ;
    }

    #endregion

    #region [ PaymentReceiptTransactionReport ]

    [Authorize(Policy = AuthPolicies.Roles.AdminOrCompanyUser)]
    [UseFiltering<PaymentReceiptTransactionFilterType>]
    [UseSorting<PaymentReceiptTransactionSortType>]
    public async Task<ReportViewModel<PaymentReceiptTransactionReportViewModel>> GetPaymentReceiptTransactions
   ([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider, [Service] IRedisCacheService cacheService,
    [Service] IHttpContextAccessor httpContext, int? pageNumber, int? pageSize)
    {
        pageNumber ??= 1;
        pageSize ??= 10;

        var query = dbContext.TransactionReadModels.Include(c => c.PaymentReceiptTransaction).Where(c => c.PaymentReceiptTransactionId.HasValue).AsQueryable();

        var roleClaim = httpContext.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Role &&
               c.Value == UserRoleType.SuperAdmin.GetValue());
        if (roleClaim == null)
        {
            var companyIdClaim = httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (companyIdClaim == null ||
            !long.TryParse(companyIdClaim.Value, out long companyId) || companyId == 0)
            {
                throw new CompanyNotFoundException(0);
            }
            else
            {
                query = query.Where(c => c.CompanyId == companyId);
            }
        }

        int totalCount = query.Count();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var data = await query.OrderByDescending(c => c.Id)
            .Select(c => new
            {
                PaymentReceiptTransaction = c.PaymentReceiptTransaction,
                Company = c.Company,
                PaymentRequest = c.PaymentRequest,
                Transaction = c,
            })
            .Skip((pageNumber.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync();

        var bankscacheData = cacheService.GetData<List<BankReadModel>>("AllBank_key");

        if (bankscacheData == null)
        {
            bankscacheData = await dbContext.BankReadModels.ToListAsync();
            cacheService.SetData("AllBank_key", bankscacheData);
        }

        var viewModels = await Task.WhenAll(

             data.Select(async entity => new PaymentReceiptTransactionReportViewModel
             {
                 Id = entity.PaymentReceiptTransaction.Id,
                 CompanyId = entity.Company.Id,
                 CompanyPersianName = entity.Company.PersianName,
                 CompanyEnglishName = entity.Company.EnglishName,
                 Amount = entity.Transaction.Amount,
                 ReceiptImage = !string.IsNullOrEmpty(entity.PaymentReceiptTransaction.ReceiptImage) ?
                 await General.GetLogo(minioProvider, entity.PaymentReceiptTransaction.ReceiptImage) : null,
                 BankId = bankscacheData.FirstOrDefault(c => c.IbanPrefix == entity.PaymentReceiptTransaction.SourceIban.Substring(4, 3))?.Id ?? 0,
                 BankLogo = await General.GetLogo(minioProvider, bankscacheData.FirstOrDefault(c => c.IbanPrefix == entity.PaymentReceiptTransaction.SourceIban.Substring(4, 3))?.Logo),
                 BankName = bankscacheData.FirstOrDefault(c => c.IbanPrefix == entity.PaymentReceiptTransaction.SourceIban.Substring(4, 3))?.Name,
                 PaymentCode = entity.PaymentRequest?.PaymentCode,
                 CompanyLogo = !string.IsNullOrEmpty(entity.Company.Logo) ? await General.GetLogo(minioProvider, entity.Company.Logo) : null,
                 CreationDate = entity.PaymentReceiptTransaction.CreationDate,
                 ReceiptDateTime = entity.PaymentReceiptTransaction.ReceiptDateTime,
                 ReferenceNumber = entity.PaymentReceiptTransaction.ReferenceNumber,
                 TransactionStatusName = General.GetPaymentReceiptTransactionStatusName(entity.PaymentReceiptTransaction.Status),
                 TransactionStatus = entity.PaymentReceiptTransaction.Status,
                 TransactionStatusCode = entity.PaymentReceiptTransaction.Status.GetValue(),
                 SourceIban = entity.PaymentReceiptTransaction.SourceIban,
                 ModificationDate = entity.PaymentReceiptTransaction.ModificationDate,
                 Description = entity.PaymentReceiptTransaction.Description
             }).ToList()
            );

        return new ReportViewModel<PaymentReceiptTransactionReportViewModel>
        {
            TotalCount = totalCount,
            CurrentPage = pageNumber.Value,
            TotalPages = totalPages,
            HasNextPage = pageNumber < totalPages,
            HasPreviousPage = pageNumber > 1,
            Models = viewModels
        }; ;
    }

    [Authorize(Policy = AuthPolicies.Roles.AdminOrCompanyUser)]
    public async Task<PaymentReceiptTransactionReportViewModel> GetPaymentReceiptTransaction
   ([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider, [Service] IRedisCacheService cacheService,
   [Service] IHttpContextAccessor httpContext, long id)
    {
        var query = dbContext.TransactionReadModels
            .Include(c => c.PaymentReceiptTransaction).Where(c => c.PaymentReceiptTransactionId.HasValue)
            .Include(c => c.Company).Include(c => c.PaymentRequest).AsQueryable();

        var roleClaim = httpContext.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Role &&
               c.Value == UserRoleType.SuperAdmin.GetValue());

        if (roleClaim == null)
        {
            var companyIdClaim = httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId");

            if (companyIdClaim == null ||
            !long.TryParse(companyIdClaim.Value, out long companyId) || companyId == 0)
            {
                throw new CompanyNotFoundException(0);
            }
            else
            {
                query = query.Where(c => c.CompanyId == companyId);
            }
        }

        var entity = query.FirstOrDefault(c => c.PaymentReceiptTransactionId == id);
        if (entity == null)
        {
            throw new Exception("IdNotFound");
        }

        var bankscacheData = cacheService.GetData<List<BankReadModel>>("AllBank_key");

        if (bankscacheData == null)
        {
            bankscacheData = await dbContext.BankReadModels.ToListAsync();
            cacheService.SetData("AllBank_key", bankscacheData);
        }
        var ibanPrefix = entity.PaymentReceiptTransaction.SourceIban.Substring(4, 3);
        var bank = bankscacheData.FirstOrDefault(c => c.IbanPrefix == ibanPrefix);
        
        
        var viewModel = new PaymentReceiptTransactionReportViewModel
        {
            Id = entity.PaymentReceiptTransaction.Id,
            CompanyId = entity.Company.Id,
            CompanyPersianName = entity.Company.PersianName,
            CompanyEnglishName = entity.Company.EnglishName,
            Amount = entity.Amount,
            ReceiptImage = !string.IsNullOrEmpty(entity.PaymentReceiptTransaction.ReceiptImage) ?
                 await General.GetLogo(minioProvider, entity.PaymentReceiptTransaction.ReceiptImage) : null,
            BankId = bank?.Id ?? 0,
            BankLogo = await General.GetLogo(minioProvider, bank?.Logo),
            BankName = bank?.Name,
            PaymentCode = entity.PaymentRequest?.PaymentCode,
            CompanyLogo = !string.IsNullOrEmpty(entity.Company.Logo) ? await General.GetLogo(minioProvider, entity.Company.Logo) : null,
            CreationDate = entity.PaymentReceiptTransaction.CreationDate,
            ReceiptDateTime = entity.PaymentReceiptTransaction.ReceiptDateTime,
            ReferenceNumber = entity.PaymentReceiptTransaction.ReferenceNumber,
            TransactionStatusName = General.GetPaymentReceiptTransactionStatusName(entity.PaymentReceiptTransaction.Status),
            TransactionStatus = entity.PaymentReceiptTransaction.Status,
            TransactionStatusCode = entity.PaymentReceiptTransaction.Status.GetValue(),
            SourceIban = entity.PaymentReceiptTransaction.SourceIban,
            ModificationDate = entity.PaymentReceiptTransaction.ModificationDate,
            Description = entity.PaymentReceiptTransaction.Description
        };
        return viewModel;
    }

    #endregion

    #region [ CharismaCardTransactionReport ]

    [Authorize(Policy = AuthPolicies.Roles.AdminOrCompanyUser)]
    [UseFiltering<CharismaCardTransactionFilterType>]
    [UseSorting<CharismaCardTransactionSortType>]
    public async Task<ReportViewModel<CharismaCardTransactionReportViewModel>> GetCharismaCardTransactions
   ([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider, [Service] IRedisCacheService cacheService,
    [Service] IHttpContextAccessor httpContext, int? pageNumber, int? pageSize)
    {
        pageNumber ??= 1;
        pageSize ??= 10;

        var query = dbContext.TransactionReadModels.Include(c => c.CharismaCardTransaction).
            Where(c => c.CharismaCardTransactionId.HasValue).AsQueryable();

        var roleClaim = httpContext.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Role &&
               c.Value == UserRoleType.SuperAdmin.GetValue());
        if (roleClaim == null)
        {
            var companyIdClaim = httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (companyIdClaim == null ||
            !long.TryParse(companyIdClaim.Value, out long companyId) || companyId == 0)
            {
                throw new CompanyNotFoundException(0);
            }
            else
            {
                query = query.Where(c => c.CompanyId == companyId);
            }
        }

        int totalCount = query.Count();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var data = await query.OrderByDescending(c => c.Id)
            .Select(c => new
            {
                CharismaCardTransaction = c.CharismaCardTransaction,
                Company = c.Company,
                PaymentRequest = c.PaymentRequest,
                Transaction = c,
            })
            .Skip((pageNumber.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync();

        var bankscacheData = cacheService.GetData<List<BankReadModel>>("AllBank_key");

        if (bankscacheData == null)
        {
            bankscacheData = await dbContext.BankReadModels.ToListAsync();
            cacheService.SetData("AllBank_key", bankscacheData);
        }

        var viewModels = await Task.WhenAll(

             data.Select(async entity => new CharismaCardTransactionReportViewModel
             {
                 Id = entity.CharismaCardTransaction.Id,
                 CompanyId = entity.Company.Id,
                 CompanyPersianName = entity.Company.PersianName,
                 CompanyEnglishName = entity.Company.EnglishName,
                 Amount = entity.Transaction.Amount,
                 PaymentCode = entity.PaymentRequest?.PaymentCode,
                 CompanyLogo = !string.IsNullOrEmpty(entity.Company.Logo) ? await General.GetLogo(minioProvider, entity.Company.Logo) : null,
                 CreationDate = entity.CharismaCardTransaction.CreationDate,
                 ReferenceNumber = entity.CharismaCardTransaction.ReferenceNumber,
                 TransactionStatusName = General.GetCharismaCardTransactionStatusName(entity.CharismaCardTransaction.Status),
                 TransactionStatus = entity.CharismaCardTransaction.Status,
                 TransactionStatusCode = entity.CharismaCardTransaction.Status.GetValue(),
                 ProviderTrackerId = entity.CharismaCardTransaction.ProviderTrackId,
                 TrackerId = entity.CharismaCardTransaction.TrackId,
                 ModificationDate = entity.CharismaCardTransaction.ModificationDate,

             }).ToList()
            );

        return new ReportViewModel<CharismaCardTransactionReportViewModel>
        {
            TotalCount = totalCount,
            CurrentPage = pageNumber.Value,
            TotalPages = totalPages,
            HasNextPage = pageNumber < totalPages,
            HasPreviousPage = pageNumber > 1,
            Models = viewModels
        };
    }
    #endregion

    #region [ TransactionReport]

    [Authorize(Policy = AuthPolicies.Roles.AdminOrCompanyUser)]
    [UseFiltering<TransactionFilterType>]
    [UseSorting<TransactionSortType>]
    public async Task<ReportViewModel<TransactionReportViewModel>> GetTransactions
([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider, [Service] IRedisCacheService cacheService,
[Service] IHttpContextAccessor httpContext, int? pageNumber, int? pageSize)
    {
        pageNumber ??= 1;
        pageSize ??= 10;

        IQueryable<TransactionReadModel> query = dbContext.TransactionReadModels;

        var roleClaim = httpContext.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Role &&
              c.Value == UserRoleType.SuperAdmin.GetValue());
        if (roleClaim == null)
        {
            var companyIdClaim = httpContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "CompanyId");
            if (companyIdClaim == null ||
            !long.TryParse(companyIdClaim.Value, out long companyId) || companyId == 0)
            {
                throw new CompanyNotFoundException(0);
            }
            else
            {
                query = query.Where(c => c.CompanyId == companyId);
            }
        }
        int totalCount = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var data = await query.OrderByDescending(c => c.Id)
         .Select(c => new
         {
             Transaction = c,
             Company = c.Company,
             PaymentRequest = c.PaymentRequest,
             Application = c.PaymentRequest.Application,
             IPGTransaction = c.IPGTransaction,
             CharismaCardTransaction = c.CharismaCardTransaction,
             PaymentReceiptTransaction = c.PaymentReceiptTransaction,
             DirectDebitTransaction = c.DirectDebitTransaction,
             CompanyIPG = c.IPGTransaction != null ? c.IPGTransaction.CompanyIPG : null,
             IPGType = c.IPGTransaction != null && c.IPGTransaction.CompanyIPG != null ? c.IPGTransaction.CompanyIPG.IPGType : null,
             Provider = c.IPGTransaction != null && c.IPGTransaction.CompanyIPG != null ? c.IPGTransaction.CompanyIPG.Provider : null,
             CompanyDeposit = c.DestinationDeposit
         })
         .Skip((pageNumber.Value - 1) * pageSize.Value)
         .Take(pageSize.Value)
         .ToListAsync();

        var UserscacheData = cacheService.GetData<List<UserReadModel>>("AllUser_key");

        if (UserscacheData == null)
        {
            UserscacheData = await dbContext.UserReadModels.ToListAsync();
            cacheService.SetData("AllUser_key", UserscacheData);
        }
        var viewModels = await Task.WhenAll(data.Select(async entity => new TransactionReportViewModel
        {
            Id = entity.Transaction.Id,
            CompanyId = entity.Company.Id,
            CompanyPersianName = entity.Company.PersianName,
            CompanyEnglishName = entity.Company.EnglishName,
            Amount = entity.Transaction.Amount,
            TransactionMethodType = entity.Transaction.TransactionMethodType,
            TransactionMethodTypeName = General.GetTransactionMethodTypeName(entity.Transaction.TransactionMethodType),
            IPGTypeId = entity.IPGType?.Id ?? 0,
            IpgTypeName = entity.IPGType?.PersianName,
            ProviderId = entity.Provider?.Id ?? 0,
            ProviderName = entity.Provider?.PersianName,
            ApplicationName = entity.Application?.PersianName,
            PaymentCode = entity.PaymentRequest?.PaymentCode,
            CompanyDepositName = entity.CompanyDeposit.Name,
            CompanyDepositaccountNumber = entity.CompanyDeposit.AccountNumber,
            CompanyDepositIban = entity.CompanyDeposit.Iban,
            CompanyLogo = !string.IsNullOrEmpty(entity.Company.Logo) ? await General.GetLogo(minioProvider, entity.Company.Logo) : null,
            TransactionCreateDateTime = entity.Transaction.CreationDate,
            TransactionModificationDateTime = entity.Transaction.ModificationDate,
            FirstName = UserscacheData.FirstOrDefault(c => c.Id == entity.Transaction.CreationUserId)?.FirstName,
            LastName = UserscacheData.FirstOrDefault(c => c.Id == entity.Transaction.CreationUserId)?.LastName,
            NationalCode = entity.PaymentRequest.NationalCode,
            ApplicationId = entity.Application?.Id ?? 0,
            ReferenceNumber = entity.IPGTransaction?.ReferenceNumber ??
                               entity.CharismaCardTransaction?.ReferenceNumber ??
                               entity.PaymentReceiptTransaction?.ReferenceNumber ??
                               entity.DirectDebitTransaction?.TrackId,
            TransactionStatusName = General.GetTransactionStatusName(entity.Transaction.Status),
            TransactionStatus = entity.Transaction.Status,
            TransactionStatusCode = entity.Transaction.Status.GetValue()
        }));

        return new ReportViewModel<TransactionReportViewModel>
        {
            TotalCount = totalCount,
            CurrentPage = pageNumber.Value,
            TotalPages = totalPages,
            HasNextPage = pageNumber < totalPages,
            HasPreviousPage = pageNumber > 1,
            Models = viewModels
        };
    }

    #endregion
}
