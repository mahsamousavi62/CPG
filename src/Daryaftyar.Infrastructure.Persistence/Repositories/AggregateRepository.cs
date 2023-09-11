using Ardalis.Specification.EntityFrameworkCore;
using Daryaftyar.Domain.SeedWork;
using Daryaftyar.Domain.SharedKernel;
using Daryaftyar.Infrastructure.Persistence.DbContexts;

namespace Daryaftyar.Infrastructure.Persistence.Repositories
{
    public class AggregateRepository<T> : RepositoryBase<T>, IAggregateReadRepository<T>, IAggregateRepository<T> where T : class, IAggregateRoot
    {
        public AggregateRepository(WriteDbContext writeDbContext)
            : base(writeDbContext)
        {
        }
    }
}