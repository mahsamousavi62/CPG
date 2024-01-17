using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class ProviderPaymentMethodReadModelConfiguration : IEntityTypeConfiguration<ProviderPaymentMethodReadModel>
{
    public void Configure(EntityTypeBuilder<ProviderPaymentMethodReadModel> readModel)
    {
        readModel.ToTable("ProviderPaymentMethod");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id).HasColumnName("Id");
        readModel.Property(x => x.ProviderId);
        readModel.Property(x => x.MethodType);
                  
    }
}
