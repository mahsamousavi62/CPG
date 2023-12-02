using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.AggregateModels.BankAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class BankProviderConfiguration : IEntityTypeConfiguration<BankProvider>
{
    public void Configure(EntityTypeBuilder<BankProvider> entity)
    {
        entity.ToTable("BankProvider");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.BankId).HasColumnName("BankId").HasColumnType("int").IsRequired();
        entity.Property(x => x.ProviderType).HasColumnName("ProviderType").HasColumnType("tinyint").IsRequired();

        entity.HasOne(x => x.Bank)
              .WithMany(x => x.BankProviders)
              .HasForeignKey(x => x.BankId);
    }
}
