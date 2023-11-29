using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;
using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CPG.Infrastructure.Persistence.DbContexts;

public class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    public IQueryable<ApplicationSettingReadModel> ApplicationSettingReadModels => Set<ApplicationSettingReadModel>().AsNoTracking();

    public IQueryable<CompanyReadModel> CompanyReadModels => Set<CompanyReadModel>().AsNoTracking();

    public IQueryable<BankReadModel> BankReadModels => Set<BankReadModel>().AsNoTracking();
    public IQueryable<CompanyDepositReadModel> CompanyDepositReadModels => Set<CompanyDepositReadModel>().AsNoTracking();

    public IQueryable<ProviderReadModel> ProviderReadModels => Set<ProviderReadModel>().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new ApplicationSettingReadModelConfiguration())
            .ApplyConfiguration(new CompanyReadModelConfiguration())
            .ApplyConfiguration(new CompanyPaymentMethodsReadModelConfiguration())
            .ApplyConfiguration(new BankReadModelConfiguration())
            .ApplyConfiguration(new CompanyDepositReadModelConfiguration())

            
         
            .ApplyConfiguration(new ProviderReadModelConfiguration());
    }
}