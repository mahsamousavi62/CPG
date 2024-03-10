using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class PaymentReceiptTransactionReadModelConfiguration : IEntityTypeConfiguration<PaymentReceiptTransactionReadModel>
{
    public void Configure(EntityTypeBuilder<PaymentReceiptTransactionReadModel> reader)
    {
        reader.ToTable("PaymentReceiptTransaction");
        reader.HasKey(x => x.Id);
        reader.Ignore(x => x.Transaction);

        reader.Property(x => x.Id);
        reader.Property(x => x.SourceIban);
        reader.Property(x => x.ReferenceNumber);
        reader.Property(x => x.ReceiptDateTime);
        reader.Property(x => x.ReceiptImage);
        reader.Property(x => x.Description);
        reader.Property(x => x.Status);
        reader.Property(x => x.IsActive);
        reader.Property(x => x.CreationDate);
        reader.Property(x => x.ModificationDate);
    }
}
