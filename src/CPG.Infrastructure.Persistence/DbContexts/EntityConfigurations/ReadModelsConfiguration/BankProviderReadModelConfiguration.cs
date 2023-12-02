using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class BankProviderReadModelConfiguration : IEntityTypeConfiguration<BankProviderReadModel>
{
    public void Configure(EntityTypeBuilder<BankProviderReadModel> readModel)
    {
        readModel.ToTable("BankProvider");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.BankId).HasColumnName("BankId");
        readModel.Property(x => x.ProviderType).HasColumnName("ProviderType");
        readModel.Property(x => x.IsActive).HasColumnName("IsActive");

        readModel.HasOne(x => x.Bank)
                 .WithMany(x => x.BankProviders)
                 .HasForeignKey(x => x.BankId);
    }
}