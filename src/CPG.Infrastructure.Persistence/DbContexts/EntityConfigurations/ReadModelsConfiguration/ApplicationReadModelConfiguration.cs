using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class ApplicationReadModelConfiguration : IEntityTypeConfiguration<ApplicationReadModel>
{
    public void Configure(EntityTypeBuilder<ApplicationReadModel> readModel)
    {
        readModel.ToTable("Application");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.PersianName).HasColumnName("PersianName");
        readModel.Property(x => x.EnglishName).HasColumnName("EnglishName");
        readModel.Property(x => x.Logo).HasColumnName("Logo");
        readModel.Property(x => x.ResponseApiUrl).HasColumnName("ResponseApiUrl");        
        readModel.Property(x => x.IsActive).HasColumnName("IsActive");
        readModel.Property(x => x.CreationDate).HasColumnName("CreationDate");
        readModel.Property(x => x.ModificationDate).HasColumnName("ModificationDate");

        readModel.HasMany(x => x.ApplicationIdentifiers)
                 .WithOne(x => x.Application)
                 .HasForeignKey(x => x.ApplicationId);

        readModel
       .HasMany(c => c.PaymentRequests)
       .WithOne(p => p.Application)
       .HasForeignKey(p => p.ApplicationId);
    }
}