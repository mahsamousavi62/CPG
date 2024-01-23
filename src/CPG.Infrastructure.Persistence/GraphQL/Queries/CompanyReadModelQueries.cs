using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.Types.Company;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class CompanyReadModelQueries
{
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
}
