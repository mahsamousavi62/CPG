using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration
{
    public class ProviderReadModelConfiguration : IEntityTypeConfiguration<ProviderReadModel>
    {
        public void Configure(EntityTypeBuilder<ProviderReadModel> readModel)
        {
            readModel.ToTable("Provider");
            readModel.HasKey(x => x.Id);
            readModel.Property(x => x.Id).HasColumnName("Id");
            readModel.Property(x => x.PersianName).HasColumnName("PersianName");
            readModel.Property(x => x.EnglishName).HasColumnName("EnglishName");
            readModel.Property(x => x.Logo).HasColumnName("Logo");
            readModel.Property(x => x.ProviderType).HasColumnName("ProviderType");
            readModel.Property(x => x.ProviderData).HasColumnName("ProviderData");
            readModel.Property(x => x.IpgVerificationTimeLimit).HasColumnName("IpgVerificationTimeLimit").HasColumnType("tinyint");            
            readModel.Property(x => x.IpgBaseUrl).HasColumnName("IpgBaseUrl");
            readModel.Property(x => x.IsActive).HasColumnName("IsActive");
            readModel.Property(x => x.CreationDate).HasColumnName("CreationDate");
            readModel.Property(x => x.ModificationDate).HasColumnName("ModificationDate");
        }
    }
}
