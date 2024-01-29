using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.GraphQL.Types.Company;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class ApplicationReadModelQueries
{
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
            ApplicationIdentifiers = app.ApplicationIdentifiers != null ? app.ApplicationIdentifiers.Select(t=> new ApplicationIdentifierReadModel
            {
                Id = t.Id,
                ApplicationId = t.ApplicationId,
                IdpClientId = t.IdpClientId
            }) : null,
            ApplicationCallbackUrls = app.ApplicationCallbackUrls != null ? app.ApplicationCallbackUrls.Select(t=>new ApplicationCallbackUrlReadModel
            {
                Id = t.Id,
                ApplicationId = t.ApplicationId,
                CallbackUrl = t.CallbackUrl,
            }) : null,
        });

        return data;
    }
}