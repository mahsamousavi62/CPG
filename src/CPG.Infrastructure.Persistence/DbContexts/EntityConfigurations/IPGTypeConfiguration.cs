using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.AggregateModels.IPGTypeAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class IPGTypeConfiguration : IEntityTypeConfiguration<IPGType>
{
    public void Configure(EntityTypeBuilder<IPGType> entity)
    {
        entity.ToTable("IPGType");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.PersianName).HasColumnName("PersianName").HasMaxLength(256).HasColumnType("nvarchar").IsRequired();
        entity.Property(x => x.EnglishName).HasColumnName("EnglishName").HasMaxLength(256).HasColumnType("varchar").IsRequired();
        entity.Property(x => x.Logo).HasColumnName("Logo").HasColumnType("nvarchar(max)").IsRequired();        
    }
}