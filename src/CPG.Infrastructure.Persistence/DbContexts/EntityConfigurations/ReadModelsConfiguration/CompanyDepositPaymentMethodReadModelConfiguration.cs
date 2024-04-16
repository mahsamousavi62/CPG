using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class CompanyDepositPaymentMethodReadModelConfiguration : IEntityTypeConfiguration<CompanyDepositPaymentMethodReadModel>
{
    public void Configure(EntityTypeBuilder<CompanyDepositPaymentMethodReadModel> readModel)
    {
        readModel.ToTable("CompanyDepositPaymentMethod");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.CompanyId);
        readModel.Property(x => x.MethodType);
    }
}