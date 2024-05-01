using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class PaymentRequestMethodReadModelConfiguration : IEntityTypeConfiguration<PaymentRequestMethodReadModel>
{
    public void Configure(EntityTypeBuilder<PaymentRequestMethodReadModel> reader)
    {
        reader.ToTable("PaymentRequestMethod");
        reader.HasKey(x => x.Id);
        reader.Property(x => x.Id);
        reader.Property(x => x.PaymentRequestId);
        reader.Property(x => x.PaymentMethodType).HasColumnType("tinyint");
        reader.Property(x => x.IsActive);
        reader.Property(x => x.ModificationDate);
        reader.Property(x => x.CreationDate);

        reader.HasOne(x => x.PaymentRequest)
            .WithMany(x => x.PaymentRequestMethods)
            .HasForeignKey(x => x.PaymentRequestId);
    }
}