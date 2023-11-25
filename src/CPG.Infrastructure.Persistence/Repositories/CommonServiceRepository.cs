using Ardalis.Specification.EntityFrameworkCore;
using CPG.Infrastructure.Persistence.DbContexts;

namespace CPG.Infrastructure.Persistence.Repositories
{
    public class CommonServiceRepository<T> : RepositoryBase<T> where T : class
    {
        public CommonServiceRepository(WriteDbContext writeDbContext)
            : base(writeDbContext)
        {
        }
    }
}
