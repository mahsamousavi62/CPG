using System.Reflection.PortableExecutable;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

public class CharismaCardTransactionReadModelConfiguration : IEntityTypeConfiguration<CharismaCardTransactionReadModel>
{
    public void Configure(EntityTypeBuilder<CharismaCardTransactionReadModel> reader)
    {
        reader.ToTable("CharismaCardTransaction");
        reader.HasKey(x => x.Id);
        reader.Ignore(x => x.Transaction);
        reader.Property(x => x.Id);
        reader.Property(x => x.ProviderTrackId);
        reader.Property(x => x.ReferenceNumber);
        reader.Property(x => x.Status);
        reader.Property(x => x.TrackId);
        reader.Property(x => x.IsActive);
        reader.Property(x => x.CreationDate);
        reader.Property(x => x.ModificationDate);
    }
}
