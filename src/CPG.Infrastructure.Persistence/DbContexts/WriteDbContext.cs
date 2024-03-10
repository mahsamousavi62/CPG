using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
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
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts;

public class WriteDbContext(DbContextOptions<WriteDbContext> options, IMediator mediator) : DbContext(options)
{
    private readonly IMediator _mediator = mediator;

    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
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
    public DbSet<PaymentRequest> PaymentRequests { get; set; }
    public DbSet<CompanyIPG> CompanyIPGs { get; set; }
    public DbSet<CompanyIPGDeposit> CompanyIPGDeposits { get; set; }
    public DbSet<CompanyShaparakSetting> ShaparakSettings { get; set; }
    public DbSet<DirectDebitGrant> DirectDebitGrants { get; set; }
    public DbSet<DirectDebitPlan> DirectDebitPlans { get; set; }
    public DbSet<PaymentReceiptTransaction> PaymentReceiptTransactions { get; set; }
    
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
            .ApplyConfiguration(new PaymentRequestConfiguration())
            .ApplyConfiguration(new ApplicationCallbackUrlConfiguration())
            .ApplyConfiguration(new TransactionConfiguration())
            .ApplyConfiguration(new IPGTransactionConfiguration())
            .ApplyConfiguration(new CompanyShaparakSettingConfiguration())
            .ApplyConfiguration(new BankDirectDebitSettingConfiguration())
            .ApplyConfiguration(new DirectDebitGrantConfiguration())
            .ApplyConfiguration(new DirectDebitPlanConfiguration())
            .ApplyConfiguration(new DirectDebitTransactionConfiguration())
            .ApplyConfiguration(new PaymentReceiptTransactionConfiguration())
        ;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await _mediator.DispatchDomainEventsAsync(this);

        return result;
    }
}
