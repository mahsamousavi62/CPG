using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<Domain.AggregateModels.ApplicationAggregate.Application>
{
    public void Configure(EntityTypeBuilder<Domain.AggregateModels.ApplicationAggregate.Application> entity)
    {
        entity.ToTable("Application");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.PersianName).HasColumnName("PersianName").HasMaxLength(256).HasColumnType("nvarchar").IsRequired();
        entity.Property(x => x.EnglishName).HasColumnName("EnglishName").HasMaxLength(256).HasColumnType("varchar").IsRequired();
        entity.Property(x => x.Logo).HasColumnName("Logo").HasColumnType("nvarchar(max)").IsRequired();
        entity.Property(x => x.ResponseApiUrl).HasColumnName("ResponseApiUrl").HasMaxLength(2048).HasColumnType("varchar").IsRequired();

        entity.HasMany(x => x.ApplicationIdentifiers)
              .WithOne(x => x.Application)
              .HasForeignKey(x => x.ApplicationId);
    }
}