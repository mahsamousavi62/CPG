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
            readModel.Property(x => x.IsActive);
            readModel.Property(x => x.ModificationDate);
            readModel.Property(x => x.CreationDate);
            readModel.HasMany(a => a.PaymentMethods)
            .WithOne(b => b.Company)
            .HasForeignKey(b => b.CompanyId);

            readModel
        .HasMany(c => c.CompanyDeposits)
        .WithOne(p => p.Company)
        .HasForeignKey(p => p.CompanyId);

         readModel
        .HasMany(c => c.PaymentMethods)
        .WithOne(p => p.Company)
        .HasForeignKey(p => p.CompanyId);

            readModel
            .HasMany(c => c.PaymentRequests)
            .WithOne(p => p.Company)
            .HasForeignKey(p => p.CompanyId);
        }
    }
}
