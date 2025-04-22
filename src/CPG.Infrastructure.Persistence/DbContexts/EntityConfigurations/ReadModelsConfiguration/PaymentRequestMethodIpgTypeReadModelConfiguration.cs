using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class PaymentRequestMethodIpgTypeReadModelConfiguration : IEntityTypeConfiguration<PaymentRequestMethodIpgTypeReadModel>
{
    public void Configure(EntityTypeBuilder<PaymentRequestMethodIpgTypeReadModel> reader)
    {
        reader.ToTable("PaymentRequestMethodIpgType");
        reader.HasKey(x => x.Id);
        reader.Property(x => x.Id);
        reader.Property(x => x.PaymentRequestMethodId);
        reader.Property(x => x.IpgTypeId);
        reader.Property(x => x.IsActive);
        reader.Property(x => x.ModificationDate);
        reader.Property(x => x.CreationDate);

        reader.HasOne(x => x.PaymentRequestMethod)
            .WithMany(x => x.PaymentRequestMethodIpgTypes)
            .HasForeignKey(x => x.PaymentRequestMethodId);

        reader.HasOne(x => x.IpgType)
            .WithMany(x => x.PaymentRequestMethodIpgTypes)
            .HasForeignKey(x => x.PaymentRequestMethodId);
    }
}