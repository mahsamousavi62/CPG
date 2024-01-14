using CPG.Domain.AggregateModels.ProviderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class ProviderPaymentMethodConfiguration : IEntityTypeConfiguration<ProviderPaymentMethod>
{
    public void Configure(EntityTypeBuilder<ProviderPaymentMethod> entity)
    {
        entity.ToTable("ProviderPaymentMethod");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.ProviderId).HasColumnName("ProviderId").IsRequired();
        entity.Property(x => x.MethodType).HasColumnName("MethodType").HasColumnType("tinyint").IsRequired();
    }
}
