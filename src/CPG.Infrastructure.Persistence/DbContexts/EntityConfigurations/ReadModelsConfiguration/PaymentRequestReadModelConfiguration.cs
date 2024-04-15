using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class PaymentRequestReadModelConfiguration : IEntityTypeConfiguration<PaymentRequestReadModel>
{
    public void Configure(EntityTypeBuilder<PaymentRequestReadModel> reader)
    {
        reader.ToTable("PaymentRequest");
        reader.HasKey(x => x.Id);
        reader.Property(x => x.Id);
        reader.Property(x => x.CompanyId);
        reader.Property(x => x.ApplicationId);
        reader.Property(x => x.DestinationDepositIban).HasColumnName("DestinationDepositIban");
        reader.Property(x => x.NationalCode);
        reader.Property(x => x.Description);
        reader.Property(x => x.Amount);
        reader.Property(x => x.CallBackUrl);
        reader.Property(x => x.PaymentCode);
        reader.Property(x => x.TrackerId);
        reader.Property(x => x.PaymentId);
        reader.Property(x => x.Status).HasColumnName("Status").HasColumnType("tinyint");
        reader.Property(x => x.IsUsed);
        reader.Property(x => x.VerificationDateTime);
        reader.Property(x => x.UrlExpirationDateTime);
        reader.Property(x => x.IsActive);
        reader.Property(x => x.ModificationDate);
        reader.Property(x => x.CreationDate);
    }
}