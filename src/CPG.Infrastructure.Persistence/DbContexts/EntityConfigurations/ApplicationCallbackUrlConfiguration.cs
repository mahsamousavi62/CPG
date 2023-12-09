using CPG.Domain.AggregateModels.ApplicationAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class ApplicationCallbackUrlConfiguration : IEntityTypeConfiguration<ApplicationCallbackUrl>
{
    public void Configure(EntityTypeBuilder<ApplicationCallbackUrl> entity)
    {
        entity.ToTable("ApplicationCallbackUrl");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.CallbackUrl).HasColumnName("CallbackUrl").HasColumnType("varchar").HasMaxLength(2048).IsRequired();
        entity.Property(x => x.ApplicationId).IsRequired();
    }
}
