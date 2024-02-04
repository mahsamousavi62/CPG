using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

internal class DirectDebitGrantConfiguration : IEntityTypeConfiguration<DirectDebitGrant>
{
    public void Configure(EntityTypeBuilder<DirectDebitGrant> entity)
    {
        entity.ToTable("DirectDebitGrant");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).UseIdentityColumn();
        entity.Property(x => x.UserId).HasColumnType("bigint").IsRequired();
        entity.Property(x => x.BankId).HasColumnType("int").IsRequired();
        entity.Property(x => x.AccountNumber).HasColumnType("varchar").HasMaxLength(255);
        entity.Property(x => x.PhoneNumber).HasColumnType("char").HasMaxLength(11).IsRequired();
        entity.Property(x => x.SuccessTransactionCountLimitPerMonth).HasColumnType("int").IsRequired();
        entity.Property(x => x.AmountLimitPerTransaction).HasColumnType("numeric").IsRequired();
        entity.Property(x => x.TrackId).HasColumnType("varchar").HasMaxLength(255);
        entity.Property(x => x.ExpirationDate).HasColumnType("datetime2").IsRequired();
        entity.Property(x => x.RevokeDateTime).HasColumnType("datetime2");
        entity.Property(x => x.ProviderId).HasColumnType("bigint").IsRequired();
        entity.Property(x => x.GrantToken).HasColumnType("varchar").HasMaxLength(255);
        entity.Property(x => x.AuthorizationId).HasColumnType("varchar").HasMaxLength(255);
        entity.Property(x => x.Status).HasColumnType("tinyint").IsRequired();

        entity.HasOne(x => x.Provider).WithMany(x => x.DirectDebitGrants).HasForeignKey(x => x.ProviderId);
    }
}