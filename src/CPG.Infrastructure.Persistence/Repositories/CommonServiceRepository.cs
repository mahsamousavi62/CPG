using Ardalis.Specification.EntityFrameworkCore;
using CPG.Infrastructure.Persistence.DbContexts;

namespace CPG.Infrastructure.Persistence.Repositories;

public class CommonServiceRepository<T>(WriteDbContext writeDbContext) : RepositoryBase<T>(writeDbContext) where T : class
{
}
