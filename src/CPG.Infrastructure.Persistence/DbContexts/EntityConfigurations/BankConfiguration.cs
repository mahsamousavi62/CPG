using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.AggregateModels.BankAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class BankConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> entity)
        {
            entity.ToTable("Bank");
            entity.HasKey(x => x.Id);

            entity.Ignore(x => x.DomainEvents);

            entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();

            entity.Property(x => x.Name).HasColumnName("Name").HasMaxLength(256).HasColumnType("nvarchar").IsUnicode(true).UseCollation("Persian_100_CI_AI").IsRequired();

            entity.Property(x => x.LogoAddress).HasColumnName("LogoAddress").HasMaxLength(256).HasColumnType("varchar").IsRequired();

            entity.Property(x => x.HasDirectDebitFeature).HasColumnName("HasDirectDebitFeature").HasColumnType("bit");

            entity.OwnsOne(x => x.IbanPrefix, x =>
            {
                x.Property(e => e.Value)
                    .HasColumnName("IbanPrefix")
                    .HasMaxLength(6)
                    .HasColumnType("varchar")
                    .IsRequired();
            });

            entity.HasMany(c => c.CompanyDeposits)
                .WithOne(p => p.Bank)
                .HasForeignKey(p => p.BankId);

            entity.HasOne(c => c.DirectDebitSetting)
                .WithOne(p => p.Bank)
                .HasForeignKey<BankDirectDebitSetting>(p => p.BankId);
        }
    }
}
