using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class CompanyIPGDepositReadModelConfiguration : IEntityTypeConfiguration<CompanyIPGDepositReadModel>
{
    public void Configure(EntityTypeBuilder<CompanyIPGDepositReadModel> readModel)
    {
        readModel.ToTable("CompanyIPGDeposit");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.CompanyIPGId).HasColumnName("CompanyIPGId");
        readModel.Property(x => x.CompanyDepositId).HasColumnName("CompanyDepositId");
        readModel.Property(x => x.IsDefault).HasColumnName("IsDefault");
        readModel.Property(x => x.IsActive);
        readModel.Property(x => x.CreationDate);
        readModel.Property(x => x.ModificationDate);
    }
}