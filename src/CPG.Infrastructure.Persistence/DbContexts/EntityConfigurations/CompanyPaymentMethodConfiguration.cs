using CPG.Domain.AggregateModels.CompanyAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class CompanyPaymentMethodConfiguration : IEntityTypeConfiguration<CompanyPaymentMethod>
    {
        public void Configure(EntityTypeBuilder<CompanyPaymentMethod> entity)
        {
            entity.ToTable("CompanyPaymentMethods");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
            entity.Property(x => x.CompanyId).IsRequired();
            entity.Property(x => x.MethodType).IsRequired();
        }
    }
}
