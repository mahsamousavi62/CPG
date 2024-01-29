using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class BankDirectDebitSettingReadModelConfiguration : IEntityTypeConfiguration<BankDirectDebitSettingReadModel>
{
    public void Configure(EntityTypeBuilder<BankDirectDebitSettingReadModel> readModel)
    {
        readModel.ToTable("BankDirectDebitSetting");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id);
        readModel.Property(x => x.BankId);
        readModel.Property(x => x.ProviderId);
        readModel.Property(x => x.DDBankCode);
        readModel.Property(x => x.AuthenticationType).HasColumnType("tinyint");
        readModel.Property(x => x.MaxWithdrawalAmountPerDay);
        readModel.Property(x => x.MaxMandateValidityDurationPerMonth).HasColumnType("tinyint");
        readModel.Property(x => x.IsActive);
        readModel.Property(x => x.CreationDate);
        readModel.Property(x => x.ModificationDate);


        readModel.HasOne(c => c.Bank)
           .WithOne(p => p.DirectDebitSetting)
           .HasForeignKey<BankDirectDebitSettingReadModel>(p => p.BankId);


        readModel.HasOne(c => c.Provider)
           .WithMany(p => p.BankDirectDebitSettings)
           .HasForeignKey(p => p.ProviderId);
    }
}