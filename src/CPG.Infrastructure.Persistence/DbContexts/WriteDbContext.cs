using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;
using CPG.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts;

public class WriteDbContext(DbContextOptions<WriteDbContext> options, IMediator mediator) : DbContext(options)
{
    private readonly IMediator _mediator = mediator;

    public DbSet<User> Users { get; set; }
    public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyPaymentMethod> CompanyPaymentMethods { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<CompanyDeposit> CompanyDeposits { get; set; }
    public DbSet<Domain.AggregateModels.ApplicationAggregate.Application> Applications { get; set; }
    public DbSet<ApplicationIdentifier> ApplicationIdentifiers { get; set; }
    public DbSet<ApplicationCallbackUrl> ApplicationCallbackUrls { get; set; }
    public DbSet<IPGType> IPGTypes { get; set; }
    public DbSet<CompanyIPG> CompanyIPGs { get; set; }
    public DbSet<CompanyIPGDeposit> CompanyIPGDeposits { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .ApplyConfiguration(new ApplicationSettingsConfiguration())
            .ApplyConfiguration(new UserConfiguration())
            .ApplyConfiguration(new UserRoleConfiguration())
            .ApplyConfiguration(new CompanyConfiguration())
            .ApplyConfiguration(new CompanyPaymentMethodConfiguration())
            .ApplyConfiguration(new BankConfiguration())
            .ApplyConfiguration(new ProviderConfiguration())
            .ApplyConfiguration(new CompanyDepositConfiguration())
            .ApplyConfiguration(new IPGTypeConfiguration())
            .ApplyConfiguration(new CompanyIPGConfiguration())
            .ApplyConfiguration(new CompanyIPGDepositConfiguration())
            .ApplyConfiguration(new ApplicationConfiguration())
            .ApplyConfiguration(new ApplicationIdentifierConfiguration())
            .ApplyConfiguration(new ApplicationCallbackUrlConfiguration())
        ;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await _mediator.DispatchDomainEventsAsync(this);

        return result;
    }
}
