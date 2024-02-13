using CPG.Domain.AggregateModels.TransactionAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class DirectDebitTransactionConfiguration : IEntityTypeConfiguration<DirectDebitTransaction>
{
    public void Configure(EntityTypeBuilder<DirectDebitTransaction> entity)
    {
        entity.ToTable("DirectDebitTransaction");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).UseIdentityColumn();
        entity.Property(x => x.DirectDebitGrantId).HasColumnType("bigint").IsRequired();
        entity.Property(x => x.Status).HasColumnType("tinyint").IsRequired();
        entity.Property(x => x.TrackId).HasColumnType("varchar(255)").IsRequired();
        entity.Property(x => x.ProviderTrackerId).HasColumnType("varchar(255)");
        entity.Property(x => x.ProviderData).HasColumnType("nvarchar(max)");

        entity.HasOne(x => x.DirectDebitGrant)
            .WithMany(x => x.DirectDebitTransactions)
            .HasForeignKey(x => x.DirectDebitGrantId);
    }
}