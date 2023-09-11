using System.Linq;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Daryaftyar.Infrastructure.Persistence.DbContexts;
using Daryaftyar.Infrastructure.Persistence.DbContexts.ReadModels;

namespace Daryaftyar.Infrastructure.Persistence.GraphQL.Queries
{
    public class BookReadModelQueries
    {
        [UsePaging(IncludeTotalCount = true, MaxPageSize = 200)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<BookReadModel> GetBooks([Service] ReadDbContext dbContext)
            => dbContext.BookReadModels.AsQueryable();
    }
}
