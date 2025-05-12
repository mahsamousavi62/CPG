using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class PaymentRequestMethodDepositReadModelConfiguration : IEntityTypeConfiguration<PaymentRequestMethodDepositReadModel>
{
    public void Configure(EntityTypeBuilder<PaymentRequestMethodDepositReadModel> reader)
    {
        reader.ToTable("PaymentRequestMethodDeposit");
        reader.HasKey(x => x.Id);
        reader.Property(x => x.Id);
        reader.Property(x => x.PaymentRequestMethodId);
        reader.Property(x => x.CompanyDepositId);
        reader.Property(x => x.IsActive);
        reader.Property(x => x.ModificationDate);
        reader.Property(x => x.CreationDate);

        reader.HasOne(x => x.PaymentRequestMethod)
            .WithMany(x => x.PaymentRequestMethodDeposits)
            .HasForeignKey(x => x.PaymentRequestMethodId);

        reader.HasOne(x => x.CompanyDeposit)
            .WithMany(x => x.PaymentRequestMethodDeposits)
            .HasForeignKey(x => x.CompanyDepositId);
    }
}