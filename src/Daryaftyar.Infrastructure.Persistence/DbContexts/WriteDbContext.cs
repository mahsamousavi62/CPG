using System.Threading;
using System.Threading.Tasks;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Infrastructure.Persistence.DbContexts.EntityConfigurations;
using Daryaftyar.Infrastructure.Persistence.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Daryaftyar.Infrastructure.Persistence.DbContexts
{
    public class WriteDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public WriteDbContext(DbContextOptions<WriteDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<DaryaftyarUser> DaryaftyarUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder
                .ApplyConfiguration(new BookConfiguration())
                .ApplyConfiguration(new DaryaftyarUserConfiguration())
                .ApplyConfiguration(new LoanConfiguration());

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            
            await _mediator.DispatchDomainEventsAsync(this);

            return result;
        }
    }
}
