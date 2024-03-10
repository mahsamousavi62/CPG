using CPG.Domain.AggregateModels.TransactionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class PaymentReceiptTransactionConfiguration : IEntityTypeConfiguration<PaymentReceiptTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentReceiptTransaction> entity)
    {
        entity.ToTable("PaymentReceiptTransaction");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).UseIdentityColumn();
        entity.Property(x => x.SourceIban).HasMaxLength(26).HasColumnType("varchar").IsRequired();
        entity.Property(x => x.ReferenceNumber).HasMaxLength(255).HasColumnType("nvarchar").IsRequired();
        entity.Property(x => x.ReceiptDateTime).HasColumnType("datetime2(7)").IsRequired();
        entity.Property(x => x.Description).HasMaxLength(1000).HasColumnType("nvarchar");
        entity.Property(x => x.ReceiptImage).HasColumnType("varchar(max)").IsRequired();
        entity.Property(x => x.Status).HasColumnType("tinyint").IsRequired();
    }
}
