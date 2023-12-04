using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class ApplicationIdentifierReadModelConfiguration : IEntityTypeConfiguration<ApplicationIdentifierReadModel>
{
    public void Configure(EntityTypeBuilder<ApplicationIdentifierReadModel> readModel)
    {
        readModel.ToTable("ApplicationIdentifier");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.ApplicationId).HasColumnName("ApplicationId");
        readModel.Property(x => x.IdpClientId).HasColumnName("IdpClientId");
    }
}