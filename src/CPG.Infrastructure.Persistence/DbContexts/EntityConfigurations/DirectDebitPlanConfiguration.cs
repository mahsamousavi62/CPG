using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class DirectDebitPlanConfiguration : IEntityTypeConfiguration<DirectDebitPlan>
{
    public void Configure(EntityTypeBuilder<DirectDebitPlan> entity)
    {
        entity.ToTable("DirectDebitPlan");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).UseIdentityColumn();
        entity.Property(x => x.DurationPerMonth).HasColumnType("tinyint").IsRequired();
    }
}
