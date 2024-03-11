using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class BankReadModelConfiguration : IEntityTypeConfiguration<BankReadModel>
{
    public void Configure(EntityTypeBuilder<BankReadModel> readModel)
    {
        readModel.ToTable("Bank");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.Name).HasColumnName("Name");
        readModel.Property(x => x.IbanPrefix).HasColumnName("IbanPrefix");
        readModel.Property(x => x.Logo).HasColumnName("LogoAddress");
        readModel.Property(x => x.HasDirectDebitFeature).HasColumnName("HasDirectDebitFeature");
        readModel.Property(x => x.IsActive);
        readModel.Property(x => x.CreationDate);
        readModel.Property(x => x.ModificationDate);

        readModel.HasMany(c => c.CompanyDeposits)
            .WithOne(p => p.Bank)
            .HasForeignKey(p => p.BankId);
        
        readModel.HasOne(c => c.DirectDebitSetting)
            .WithOne(p => p.Bank)
            .HasForeignKey<BankDirectDebitSettingReadModel>(p => p.BankId);
    }
}
