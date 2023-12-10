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
            entity.Ignore(x => x.CompanyIPG);
            entity.Ignore(x => x.CompanyDeposit);

            entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
            entity.Property(x => x.CompanyIPGId).HasColumnName("CompanyIPGId").IsRequired();
            entity.Property(x => x.CompanyDepositId).HasColumnName("CompanyDepositId").IsRequired();
            entity.Property(x => x.IsDefault).HasColumnName("IsDefault").HasColumnType("bit").IsRequired();
        }
    }
}
