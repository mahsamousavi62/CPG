using System;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class CharismaCardTransactionConfiguration : IEntityTypeConfiguration<CharismaCardTransaction>
{
    public void Configure(EntityTypeBuilder<CharismaCardTransaction> entity)
    {
        entity.ToTable("CharismaCardTransaction");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).UseIdentityColumn();
        entity.Property(x => x.TrackId).HasColumnName("TrackId").HasMaxLength(255).HasColumnType("nvarchar").IsRequired();
        entity.Property(x => x.ProviderTrackId).HasColumnName("ProviderTrackId").HasMaxLength(255).HasColumnType("nvarchar").IsRequired();
        entity.Property(x => x.ReferenceNumber).HasColumnName("ReferenceNumber").HasMaxLength(255).HasColumnType("nvarchar").IsRequired();
        entity.Property(x => x.Status).HasColumnName("Status").HasColumnType("tinyint").IsRequired();
    }
}
