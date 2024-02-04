using CPG.Domain.AggregateModels.BankAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class BankDirectDebitSettingConfiguration : IEntityTypeConfiguration<BankDirectDebitSetting>
{
    public void Configure(EntityTypeBuilder<BankDirectDebitSetting> entity)
    {
        entity.ToTable("BankDirectDebitSetting");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);

        entity.Property(x => x.Id).UseIdentityColumn();

        entity.Property(x => x.BankId).HasColumnType("int").IsRequired();

        entity.Property(x => x.ProviderId).HasColumnType("bigint").IsRequired();

        entity.Property(x => x.DDBankCode).HasMaxLength(255).HasColumnType("varchar").IsRequired();

        entity.Property(x => x.MaxWithdrawalAmountPerDay).HasColumnType("numeric").HasPrecision(18, 0).IsRequired();

        entity.Property(x => x.AuthenticationType).HasColumnType("tinyint").IsRequired();

        entity.Property(x => x.MaxMandateValidityDurationPerMonth).HasColumnType("tinyint").IsRequired();
    }
}
