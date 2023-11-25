using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration
{
    public class CompanyReadModelConfiguration : IEntityTypeConfiguration<CompanyReadModel>
    {
        public void Configure(EntityTypeBuilder<CompanyReadModel> readModel)
        {
            readModel.ToTable("Company");
            readModel.HasKey(x => x.Id);
            readModel.Property(x => x.Id).HasColumnName("Id");
            readModel.Property(x => x.PersianName).HasColumnName("PersianName");
            readModel.Property(x => x.EnglishName).HasColumnName("EnglishName");
            readModel.Property(x => x.Logo).HasColumnName("Logo");

            readModel.HasMany(a => a.PaymentMethods)
            .WithOne(b => b.Company)
            .HasForeignKey(b => b.CompanyId);
        }
    }
}
