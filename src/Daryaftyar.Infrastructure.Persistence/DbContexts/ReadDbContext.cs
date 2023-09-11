using System.Linq;
using Daryaftyar.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;
using Daryaftyar.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Daryaftyar.Infrastructure.Persistence.DbContexts
{
    public class ReadDbContext : DbContext
    {
        public IQueryable<BookReadModel> BookReadModels => Set<BookReadModel>().AsNoTracking();

        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfiguration(new BookReadModelConfiguration());
    }
}