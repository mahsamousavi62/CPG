using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

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
            entity.Property(x => x.SiteAddress).HasColumnName("SiteAddress").HasColumnType("varchar(1000)").IsRequired();
            entity.Property(x => x.IpgRedirectionMethodType).HasColumnName("IpgRedirectionMethodType").HasColumnType("smallint").IsRequired();


            entity
         .HasMany(c => c.PaymentMethods)
         .WithOne(p => p.Company)
         .HasForeignKey(p => p.CompanyId);

            entity
           .HasMany(c => c.Users)
           .WithOne(p => p.Company)
           .HasForeignKey(p => p.CompanyId);

            entity
             .HasMany(c => c.CompanyDeposits)
             .WithOne(p => p.Company)
             .HasForeignKey(p => p.CompanyId);

            entity
            .HasMany(x => x.PaymentRequests)
            .WithOne(x => x.Company)
            .HasForeignKey(x => x.CompanyId);
        }
    }
}
