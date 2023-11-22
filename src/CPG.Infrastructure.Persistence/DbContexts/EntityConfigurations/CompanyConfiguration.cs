using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> entity)
        {
            entity.ToTable("Company");
            entity.HasKey(x => x.Id);

            entity.Ignore(x => x.DomainEvents);
            entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
            entity.Property(x => x.NationalCodeMatchingRequied).HasColumnName("NationalCodeMatchingRequied").HasColumnType("bit").IsRequired();
            entity.Property(x => x.PersianName).HasColumnName("PersianName").HasMaxLength(256).HasColumnType("nvarchar").IsRequired();
            entity.Property(x => x.EnglishName).HasColumnName("EnglishName").HasMaxLength(256).HasColumnType("varchar").IsRequired();
            entity.Property(x => x.Logo).HasColumnName("Logo").HasColumnType("nvarchar(max)").IsRequired();

           // entity.HasMany<CompanyPaymentMethods>();
        }
    }
}
