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

    public IQueryable<UserReadModel> UserReadModels => Set<UserReadModel>().AsNoTracking();

    public IQueryable<ApplicationReadModel> ApplicationReadModels => Set<ApplicationReadModel>().AsNoTracking();

    public IQueryable<ApplicationIdentifierReadModel> ApplicationIdentifierReadModels => Set<ApplicationIdentifierReadModel>().AsNoTracking();

    public IQueryable<IPGTypeReadModel> IPGTypeReadModels => Set<IPGTypeReadModel>().AsNoTracking();

    public IQueryable<PaymentRequestReadModel> PaymentRequestReadModels=> Set<PaymentRequestReadModel>().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new ApplicationSettingReadModelConfiguration())
            .ApplyConfiguration(new CompanyReadModelConfiguration())
            .ApplyConfiguration(new CompanyPaymentMethodsReadModelConfiguration())
            .ApplyConfiguration(new BankReadModelConfiguration())
            .ApplyConfiguration(new CompanyDepositReadModelConfiguration())
            .ApplyConfiguration(new UserReadModelConfiguration())
            .ApplyConfiguration(new ProviderReadModelConfiguration())
            .ApplyConfiguration(new ApplicationReadModelConfiguration())
            .ApplyConfiguration(new ApplicationIdentifierReadModelConfiguration())
            .ApplyConfiguration(new IPGTypeReadModelConfiguration())
            .ApplyConfiguration(new PaymentRequestReadModelConfiguration());
    }
}