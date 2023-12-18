using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class UserRoleReadModelConfiguration : IEntityTypeConfiguration<UserRoleReadModel>
{
    public void Configure(EntityTypeBuilder<UserRoleReadModel> readModel)
    {
        readModel.ToTable("UserRole");
        readModel.HasKey(x => x.Id);

        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.RoleType).HasColumnName("RoleType");
        readModel.Property(x => x.UserId).HasColumnName("UserId");
        readModel.Property(x => x.IsActive).HasColumnName("IsActive");
        readModel.Property(x => x.CreationDate).HasColumnName("CreationDate");
        readModel.Property(x => x.ModificationDate).HasColumnName("ModificationDate");

        readModel.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
    }
}