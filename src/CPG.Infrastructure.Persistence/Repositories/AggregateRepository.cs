using Ardalis.Specification.EntityFrameworkCore;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;

namespace CPG.Infrastructure.Persistence.Repositories
{
    public class AggregateRepository<T> : RepositoryBase<T>, IAggregateReadRepository<T>, IAggregateRepository<T> where T : class, IAggregateRoot
    {
        public AggregateRepository(WriteDbContext writeDbContext)
            : base(writeDbContext)
        {
        }
    }
}