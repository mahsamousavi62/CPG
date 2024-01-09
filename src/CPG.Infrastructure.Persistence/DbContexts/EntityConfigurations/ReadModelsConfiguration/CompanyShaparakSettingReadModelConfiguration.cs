using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

internal class CompanyShaparakSettingReadModelConfiguration : IEntityTypeConfiguration<CompanyShaparakSettingReadModel>
{
    public void Configure(EntityTypeBuilder<CompanyShaparakSettingReadModel> readModel)
    {
        readModel.ToTable("CompanyShaparakSetting");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.CompanyId);
        readModel.Property(x => x.Key);
        readModel.Property(x => x.Iv);
        readModel.Property(x => x.ThirdPartyCode);
    }
}