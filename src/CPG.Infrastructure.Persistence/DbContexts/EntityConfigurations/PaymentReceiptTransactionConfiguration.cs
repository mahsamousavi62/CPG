
using CPG.Domain.AggregateModels.PaymentReceiptAggregate;
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
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.SourceIban).HasColumnName("SourceIban").HasMaxLength(255).HasColumnType("nvarchar").IsRequired();
        entity.Property(x => x.ReceiptDateTime).HasColumnName("ReceiptDateTime").HasColumnType("datetime2(7)").IsRequired();
        entity.Property(x => x.Description).HasColumnName("Description").HasMaxLength(1000).HasColumnType("nvarchar").IsRequired();
        entity.Property(x => x.ReceiptImage).HasColumnName("ReceiptImage").HasColumnType("nvarchar(max)").IsRequired();
        entity.Property(x => x.Status).HasColumnName("Status").HasColumnType("tinyint").IsRequired();
    }
}
