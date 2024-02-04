using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class DirectDebitPlanReadModelConfiguration : IEntityTypeConfiguration<DirectDebitPlanReadModel>
{
    public void Configure(EntityTypeBuilder<DirectDebitPlanReadModel> readModel)
    {
        readModel.ToTable("DirectDebitPlan");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id);
        readModel.Property(x => x.DurationPerMonth);
        readModel.Property(x => x.IsActive);
        readModel.Property(x => x.CreationDate);
        readModel.Property(x => x.ModificationDate);
    }
}