using CPG.Application.UseCases.CompanyDeposits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class CompanyDepositConfiguration : IEntityTypeConfiguration<CompanyDeposit>
    {
        public void Configure(EntityTypeBuilder<CompanyDeposit> entity)
        {
            entity.ToTable("CompanyDeposit");
            entity.HasKey(x => x.Id);

            entity.Ignore(x => x.DomainEvents);
            entity.Ignore(x => x.CompanyIPGDeposits);

            entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
            entity.Property(x => x.Name).HasColumnName("Name").HasMaxLength(255).HasColumnType("nvarchar").IsRequired();
            entity.Property(x => x.Iban).HasColumnName("Iban").HasMaxLength(26).HasColumnType("varchar").IsRequired();
            entity.Property(x => x.BankId).HasColumnName("BankId").HasColumnType("int").IsRequired();
            entity.Property(x => x.AccountNumber).HasColumnName("AccountNumber").HasMaxLength(255).HasColumnType("varchar").IsRequired();
            entity.Property(x => x.CompanyId).HasColumnName("CompanyId").HasColumnType("bigint").IsRequired();
            entity.Property(x => x.IsDefaultForDirectDebit).HasColumnName("IsDefaultForDirectDebit").HasColumnType("bit");
        }
    }
}
