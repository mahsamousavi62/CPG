using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;
using CPG.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts
{
    public class WriteDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public WriteDbContext(DbContextOptions<WriteDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<CPGUser> CPGUsers { get; set; }
        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .ApplyConfiguration(new BookConfiguration())
                .ApplyConfiguration(new CPGUserConfiguration())
                .ApplyConfiguration(new LoanConfiguration())
                .ApplyConfiguration(new ApplicationSettingsConfiguration());

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            await _mediator.DispatchDomainEventsAsync(this);

            return result;
        }
    }
}
