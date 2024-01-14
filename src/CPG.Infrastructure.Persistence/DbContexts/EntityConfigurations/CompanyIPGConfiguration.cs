using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class CompanyIPGConfiguration : IEntityTypeConfiguration<CompanyIPG>
{
    public void Configure(EntityTypeBuilder<CompanyIPG> entity)
    {
        entity.ToTable("CompanyIPG");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.CompanyId).IsRequired();
        entity.Property(x => x.IPGTypeId).IsRequired();
        entity.Property(x => x.ProviderData).HasColumnType("varchar(max)").IsRequired();

        entity.HasMany(c => c.IPGDeposits)
            .WithOne(p => p.CompanyIPG)
            .HasForeignKey(p => p.CompanyIPGId);

        //entity.HasMany(c => c.IPGTransactions)
        //    .WithOne(i => i.CompanyIPG)
        //    .HasForeignKey(i => i.CompanyIPG);
    }
}
