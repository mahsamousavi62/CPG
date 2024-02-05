using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.DbContexts;
using System.Linq;
using CPG.Infrastructure.Persistence.GraphQL.Types.Company;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;


public class CompanyUserModelQueries
{
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
}
