using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class CompanyPaymentMethodsReadModelConfiguration : IEntityTypeConfiguration<CompanyPaymentMethodsReadModel>
{
    public void Configure(EntityTypeBuilder<CompanyPaymentMethodsReadModel> readModel)
    {
        readModel.ToTable("CompanyPaymentMethods");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.CompanyId);
        readModel.Property(x => x.MethodType);
                  
    }
}
