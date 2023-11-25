using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

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

        readModel
            .HasOne<Company>()
            .WithOne()
            .HasForeignKey<Company>(x => x.Id);

        readModel.HasMany(x => x.PaymentMethods)
            .WithOne()
            .HasForeignKey(x => x.CompanyId);
    }
}
