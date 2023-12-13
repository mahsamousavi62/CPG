using CPG.Domain.AggregateModels.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("User");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);

        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.IDPId).HasColumnName("IDPId").HasColumnType("varchar(255)");
        entity.Property(x => x.NationalCode).HasColumnName("NationalCode").HasColumnType("char(10)").IsRequired();
        entity.Property(x => x.PhoneNumber).HasColumnName("PhoneNumber").HasColumnType("char(11)").IsRequired();
        entity.Property(x => x.FirstName).HasColumnName("FirstName").HasColumnType("nvarchar(255)").IsRequired();
        entity.Property(x => x.LastName).HasColumnName("LastName").HasColumnType("nvarchar(255)").IsRequired();
        entity.Property(x => x.IsLegal).HasColumnName("IsLegal").HasColumnType("bit");
        entity.Property(x => x.CompanyId).HasColumnName("CompanyId");
        entity.Property(x => x.KYCStatus).HasColumnName("KYCStatus").HasColumnType("tinyint");
        entity.Property(x => x.LastUpdateFromIDP).HasColumnName("LastUpdateFromIDP");

        entity.HasIndex(x => x.NationalCode).IsUnique();
        entity.HasMany(x => x.UserRoles).WithOne(x => x.User).HasForeignKey(x => x.UserId);
        entity.HasOne(x => x.Company).WithMany(x => x.Users).HasForeignKey(x => x.CompanyId);
    }
}