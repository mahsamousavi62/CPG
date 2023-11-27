using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;
using CPG.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts;

public class WriteDbContext(DbContextOptions<WriteDbContext> options, IMediator mediator) : DbContext(options)
{
    private readonly IMediator _mediator = mediator;

    public DbSet<Book> Books { get; set; }
    public DbSet<CPGUser> CPGUsers { get; set; }
    public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Bank> Banks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder
            .ApplyConfiguration(new BookConfiguration())
            .ApplyConfiguration(new CPGUserConfiguration())
            .ApplyConfiguration(new LoanConfiguration())
            .ApplyConfiguration(new ApplicationSettingsConfiguration())
            .ApplyConfiguration(new UserConfiguration())
            .ApplyConfiguration(new UserRoleConfiguration())
            .ApplyConfiguration(new CompanyConfiguration())
            .ApplyConfiguration(new CompanyPaymentMethodsConfiguration())
            .ApplyConfiguration(new BankConfiguration())
        ;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await _mediator.DispatchDomainEventsAsync(this);

        return result;
    }
}
