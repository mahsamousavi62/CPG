using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.DbContexts;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using System.Linq;
using CPG.Infrastructure.Persistence.GraphQL.Types.Bank;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class BankReadModelQueries
{
    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<BankFilterType>]
    [UseSorting<BankSortType>]
    public IQueryable<BankReadModel> GetCompanies([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider)
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
}