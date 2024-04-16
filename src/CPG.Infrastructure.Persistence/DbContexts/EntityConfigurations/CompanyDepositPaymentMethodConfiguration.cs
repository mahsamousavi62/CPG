using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class CompanyDepositPaymentMethodConfiguration : IEntityTypeConfiguration<CompanyDepositPaymentMethod>
{
    public void Configure(EntityTypeBuilder<CompanyDepositPaymentMethod> entity)
    {
        entity.ToTable("CompanyPaymentMethods");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).UseIdentityColumn();
        entity.Property(x => x.CompanyDepositId).IsRequired();
        entity.Property(x => x.MethodType).HasColumnType("tinyint").IsRequired();
    }
}