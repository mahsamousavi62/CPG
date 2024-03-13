using Azure.Core;
using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.Types.Company;
using CPG.Infrastructure.Persistence.GraphQL.Types.CompanyDeposit;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class CompanyIPGReadModelQueries
{
    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<CompanyIPGFilterType>]
    [UseSorting<CompanayIPGSortType>]
    public IQueryable<CompanyIPGReadModel> GetCompanyIPGs([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider, long companyId)
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
            //CompanyIPGDeposits = entity.CompanyIPGDeposits.
            //Select(t => new CompanyIPGDepositReadModel { Id = t.Id, AccountNumber = t.CompanyDeposit.AccountNumber, Name = t.CompanyDeposit.Name }).ToList(),
            CreationDate = entity.CreationDate,
            ModificationDate = entity.ModificationDate,
            IsActive = entity.IsActive,
            //DefaultDeposit = entity.CompanyIPGDeposits.Where(t => t.IsDefault == true).
            //Select(t => new CompanyIPGDepositReadModel { Id = t.Id, AccountNumber = t.CompanyDeposit.AccountNumber, Name = t.CompanyDeposit.Name }).FirstOrDefault(),
            //IPGTypeLogo = minioProvider.PresignedGetObject(entity.IPGType.Logo).GetAwaiter().GetResult(),
            //IPGTypeName = entity.IPGType.PersianName,
            //ProviderName = entity.Provider.PersianName,
        }); ;;
        return viewModels;
    }
}
