using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class DirectDebitTransactionReadModelConfiguration : IEntityTypeConfiguration<DirectDebitTransactionReadModel>
{
    public void Configure(EntityTypeBuilder<DirectDebitTransactionReadModel> entity)
    {
        entity.ToTable("DirectDebitTransaction");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.Transaction);
        entity.Property(x => x.Id);
        entity.Property(x => x.DirectDebitGrantId);
        entity.Property(x => x.Status);
        entity.Property(x => x.TrackId);        
        entity.Property(x => x.ProviderTrackerId);
        entity.Property(x => x.ProviderData);
        
        entity.HasOne(x => x.DirectDebitGrant)
            .WithMany(x => x.DirectDebitTransactions)
            .HasForeignKey(x => x.DirectDebitGrantId);
    }
}