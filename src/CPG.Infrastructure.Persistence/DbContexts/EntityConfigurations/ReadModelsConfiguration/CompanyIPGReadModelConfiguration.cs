using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class CompanyIPGReadModelConfiguration : IEntityTypeConfiguration<CompanyIPGReadModel>
{
    public void Configure(EntityTypeBuilder<CompanyIPGReadModel> readModel)
    {
        readModel.ToTable("CompanyIPG");

        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.CompanyId).HasColumnName("CompanyId");
        readModel.Property(x => x.ProviderId).HasColumnName("ProviderId");
        readModel.Property(x => x.IPGTypeId).HasColumnName("IPGTypeId");
        readModel.Property(x => x.VerificationTimeLimit).HasColumnName("VerificationTimeLimit");
        readModel.Property(x => x.ProviderData).HasColumnName("ProviderData");
        readModel.Property(x => x.IsActive);
        readModel.Property(x => x.CreationDate);
        readModel.Property(x => x.ModificationDate);

        readModel.HasMany(x => x.CompanyIPGDeposits)
                 .WithOne(x => x.CompanyIPG)
                 .HasForeignKey(b => b.CompanyIPGId);
    }
}
