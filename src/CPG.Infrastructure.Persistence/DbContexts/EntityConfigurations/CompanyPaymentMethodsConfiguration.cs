using CPG.Domain.AggregateModels.CompanyAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class CompanyPaymentMethodsConfiguration : IEntityTypeConfiguration<CompanyPaymentMethods>
    {
        public void Configure(EntityTypeBuilder<CompanyPaymentMethods> entity)
        {
            entity.ToTable("CompanyPaymentMethods");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
            entity.Property(x => x.CompanyId).IsRequired();
            entity.Property(x => x.MethodType).IsRequired();
        }
    }
}
