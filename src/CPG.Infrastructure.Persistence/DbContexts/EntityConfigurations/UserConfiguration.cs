using CPG.Domain.AggregateModels.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("User");
            entity.HasKey(x => x.Id);

            entity.Ignore(x => x.DomainEvents);

            entity.Property(x => x.Id)
                .HasColumnName("Id")
                .UseIdentityColumn();

            entity.Property(x => x.IDPId)
              .HasColumnName("IDPId")
              .IsRequired();

            entity.Property(x => x.NationalCode)
                     .HasColumnName("NationalCode").HasColumnType("char(10)")
                     .IsRequired();

            entity.Property(x => x.PhoneNumber)
                      .HasColumnName("PhoneNumber").HasColumnType("char(11)").IsRequired();

            entity.Property(x => x.FirstName)
                .HasColumnName("FirstName")
                .IsRequired();

            entity.Property(x => x.LastName)
                .HasColumnName("LastName")
                .IsRequired();

            entity.Property(x => x.IsActive)
                .HasColumnName("IsActive");

            entity.Property(x => x.IsLegal)
              .HasColumnName("IsLegal");

            entity.Property(x => x.CompanyId)
               .HasColumnName("CompanyId");

            entity.Property(x => x.KYCStatus)
               .HasColumnName("KYCStatus");

            entity.Property(x => x.CreatationDateTime)
               .HasColumnName("CreatationDateTime");

            entity.Property(x => x.LastUpdateFromIDP)
               .HasColumnName("LastUpdateFromIDP");

            entity.Property(x => x.ModificationDate)
          .HasColumnName("ModificationDate");

            entity.HasMany(x => x.userRoles);

        }
    }
}