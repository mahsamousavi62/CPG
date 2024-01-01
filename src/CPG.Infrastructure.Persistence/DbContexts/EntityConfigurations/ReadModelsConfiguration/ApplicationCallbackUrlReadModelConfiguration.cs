using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class ApplicationCallbackUrlReadModelConfiguration : IEntityTypeConfiguration<ApplicationCallbackUrlReadModel>
{
    public void Configure(EntityTypeBuilder<ApplicationCallbackUrlReadModel> readModel)
    {
        readModel.ToTable("ApplicationCallbackUrl");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.ApplicationId).HasColumnName("ApplicationId");
        readModel.Property(x => x.CallbackUrl).HasColumnName("CallbackUrl");
    }
}