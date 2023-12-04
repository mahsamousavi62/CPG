using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using System.Linq;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class CompanyReadModelQueries
{
    [UsePaging(IncludeTotalCount = true, MaxPageSize = 200)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<CompanyReadModel> GetCompanies([Service] ReadDbContext dbContext)
        => dbContext.CompanyReadModels.AsQueryable();
}
