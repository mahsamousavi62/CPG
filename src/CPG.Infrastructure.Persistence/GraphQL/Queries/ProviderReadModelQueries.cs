using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.DbContexts;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate;
using System.Linq;
using CPG.Infrastructure.Persistence.GraphQL.Types.Provider;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class ProviderReadModelQueries
{
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
            PaymentMethods = x.PaymentMethods.Select(p => new ProviderPaymentMethodReadModel
            {
                Id = p.Id,
                MethodType = p.MethodType,
                ProviderId = p.ProviderId,
            })
        });

        return data;
    }
}