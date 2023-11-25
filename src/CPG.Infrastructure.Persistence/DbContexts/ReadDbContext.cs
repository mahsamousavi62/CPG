using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;
using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CPG.Infrastructure.Persistence.DbContexts;

public class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    public IQueryable<BookReadModel> BookReadModels => Set<BookReadModel>().AsNoTracking();

    public IQueryable<ApplicationSettingReadModel> ApplicationSettingReadModels => Set<ApplicationSettingReadModel>().AsNoTracking();

    public IQueryable<CompanyReadModel> CompanyReadModels => Set<CompanyReadModel>().AsNoTracking();

        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new BookReadModelConfiguration())
               .ApplyConfiguration(new ApplicationSettingReadModelConfiguration())
               .ApplyConfiguration(new CompanyReadModelConfiguration())
               .ApplyConfiguration(new CompanyPaymentMethodsReadModelConfiguration());
        }
    }
}