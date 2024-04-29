using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class ApplicationSettingReadModelConfiguration : IEntityTypeConfiguration<ApplicationSettingReadModel>
{
    public void Configure(EntityTypeBuilder<ApplicationSettingReadModel> readModel)
    {
        readModel.ToTable("ApplicationSetting");
        readModel.HasKey(x => x.Id);

        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.EntityType).HasColumnName("Entity_Type");
        readModel.Property(x => x.Key).HasColumnName("Key");
        readModel.Property(x => x.Value).HasColumnName("Value");
    }
}
