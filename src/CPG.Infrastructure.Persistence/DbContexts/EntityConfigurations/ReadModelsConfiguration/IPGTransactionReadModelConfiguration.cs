using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class IPGTransactionReadModelConfiguration : IEntityTypeConfiguration<IPGTransactionReadModel>
{
    public void Configure(EntityTypeBuilder<IPGTransactionReadModel> entity)
    {
        entity.ToTable("IPGTransaction");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.Transaction);
        entity.Property(x => x.Id).HasColumnName("Id");
        entity.Property(x => x.TrackId).HasColumnName("TrackId");
        entity.Property(x => x.Status).HasColumnName("Status");
        entity.Property(x => x.CompanyIPGId).HasColumnName("CompanyIPGId");
        entity.Property(x => x.IPGToken).HasColumnName("IPGToken");
        entity.Property(x => x.ProviderTrackerId).HasColumnName("ProviderTrackerId");
        entity.Property(x => x.ReferenceNumber).HasColumnName("ReferenceNumber");
        entity.Property(x => x.EncryptCardNumber).HasColumnName("EncryptCardNumber");
        entity.Property(x => x.VerificationTimeLimit).HasColumnName("VerificationTimeLimit");
        entity.Property(x => x.PredicateDateTime).HasColumnName("PredicateDateTime");
    }
}