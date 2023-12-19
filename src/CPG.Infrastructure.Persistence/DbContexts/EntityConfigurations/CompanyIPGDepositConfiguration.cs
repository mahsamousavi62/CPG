using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class CompanyIPGDepositConfiguration : IEntityTypeConfiguration<CompanyIPGDeposit>
    {
        public void Configure(EntityTypeBuilder<CompanyIPGDeposit> entity)
        {
            entity.ToTable("CompanyIPGDeposit");
            entity.HasKey(x => x.Id);

            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
            entity.Property(x => x.CompanyIPGId).HasColumnName("CompanyIPGId").IsRequired();
            entity.Property(x => x.CompanyDepositId).HasColumnName("CompanyDepositId").IsRequired();
            entity.Property(x => x.IsDefault).HasColumnName("IsDefault").HasColumnType("bit").IsRequired();

            entity.HasOne(x => x.CompanyDeposit)
                .WithMany(x => x.CompanyIPGDeposits)
                .HasForeignKey(x => x.CompanyDepositId);
        }
    }
}
