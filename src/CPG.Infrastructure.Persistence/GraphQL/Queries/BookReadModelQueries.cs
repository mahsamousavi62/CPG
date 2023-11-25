using System.Linq;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class BookReadModelQueries
{
    [UsePaging(IncludeTotalCount = true, MaxPageSize = 200)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<BookReadModel> GetBooks([Service] ReadDbContext dbContext)
        => dbContext.BookReadModels.AsQueryable();
}
