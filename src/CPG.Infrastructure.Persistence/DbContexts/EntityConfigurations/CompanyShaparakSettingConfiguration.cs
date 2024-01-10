using CPG.Domain.AggregateModels.CompanyAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

internal class CompanyShaparakSettingConfiguration : IEntityTypeConfiguration<CompanyShaparakSetting>
{
    public void Configure(EntityTypeBuilder<CompanyShaparakSetting> entity)
    {
        entity.ToTable("CompanyShaparakSetting");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.CompanyId).IsRequired();
        entity.Property(x => x.Key).HasColumnType("varchar").HasMaxLength(255);
        entity.Property(x => x.Iv).HasColumnType("varchar").HasMaxLength(255);
        entity.Property(x => x.ThirdPartyCode).HasColumnType("int");
    }
}
