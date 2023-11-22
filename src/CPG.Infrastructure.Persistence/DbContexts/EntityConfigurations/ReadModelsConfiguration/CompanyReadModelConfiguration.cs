using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.SeedWork;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration
{
    public class CompanyReadModelConfiguration : IEntityTypeConfiguration<CompanyReadModel>
    {
        public void Configure(EntityTypeBuilder<CompanyReadModel> entity)
        {
            entity.ToTable("Company");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
            entity.Property(x => x.PersianName).HasColumnName("PersianName").IsRequired();
            entity.Property(x => x.EnglishName).HasColumnName("EnglishName").IsRequired();
            entity.Property(x => x.Logo).HasColumnName("Logo").IsRequired();

            entity.HasMany<CompanyPaymentMethods>();
        }
    }
}
