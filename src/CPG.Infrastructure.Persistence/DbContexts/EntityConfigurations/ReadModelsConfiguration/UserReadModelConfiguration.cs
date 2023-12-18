using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class UserReadModelConfiguration : IEntityTypeConfiguration<UserReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> readModel)
    {
        readModel.ToTable("User");
        readModel.HasKey(x => x.Id);

        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.IDPId).HasColumnName("IDPId");
        readModel.Property(x => x.NationalCode).HasColumnName("NationalCode");
        readModel.Property(x => x.PhoneNumber).HasColumnName("PhoneNumber");
        readModel.Property(x => x.FirstName).HasColumnName("FirstName");
        readModel.Property(x => x.LastName).HasColumnName("LastName");
        readModel.Property(x => x.IsActive).HasColumnName("IsActive");
        readModel.Property(x => x.IsLegal).HasColumnName("IsLegal");
        readModel.Property(x => x.CompanyId).HasColumnName("CompanyId");
        readModel.Property(x => x.KYCStatus).HasColumnName("KYCStatus").HasColumnType("tinyint");
        readModel.Property(x => x.LastUpdateFromIDP).HasColumnName("LastUpdateFromIDP");
        readModel.Property(x => x.CreationDate).HasColumnName("CreationDate");
        readModel.Property(x => x.ModificationDate).HasColumnName("ModificationDate");

        readModel.HasMany(x => x.UserRoles).WithOne(x => x.User).HasForeignKey(x => x.UserId);
    }
}
