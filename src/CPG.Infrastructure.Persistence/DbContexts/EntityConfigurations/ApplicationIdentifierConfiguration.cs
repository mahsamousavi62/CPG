using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.AggregateModels.ApplicationAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class ApplicationIdentifierConfiguration : IEntityTypeConfiguration<ApplicationIdentifier>
{
    public void Configure(EntityTypeBuilder<ApplicationIdentifier> entity)
    {
        entity.ToTable("ApplicationIdentifier");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.ApplicationId).IsRequired();
        entity.Property(x => x.IdpClientId).IsRequired();
    }
}