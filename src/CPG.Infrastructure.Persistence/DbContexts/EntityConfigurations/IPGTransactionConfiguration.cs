using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class IPGTransactionConfiguration : IEntityTypeConfiguration<IPGTransaction>
{
    public void Configure(EntityTypeBuilder<IPGTransaction> entity)
    {
        entity.ToTable("IPGTransaction");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Ignore(x => x.Transaction);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.TrackId).HasColumnName("TrackId").HasColumnType("varchar(255)").IsRequired();
        entity.Property(x => x.Status).HasColumnName("Status").HasColumnType("smallint").IsRequired();
        entity.Property(x => x.CompanyIPGId).HasColumnName("CompanyIPGId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.IPGToken).HasColumnName("IPGToken").HasColumnType("varchar(255)").IsRequired();
        entity.Property(x => x.ProviderTrackerId).HasColumnName("ProviderTrackerId").HasColumnType("nvarchar(255)").IsRequired();
        entity.Property(x => x.ReferenceNumber).HasColumnName("ReferenceNumber").HasColumnType("varchar(255)").IsRequired();
        entity.Property(x => x.EncryptCardNumber).HasColumnName("EncryptCardNumber").HasColumnType("varchar(255)").IsRequired();
        entity.Property(x => x.VerificationTimeLimit).HasColumnName("VerificationTimeLimit").HasColumnType("int");
        entity.Property(x => x.PredicateDateTime).HasColumnName("PredicateDateTime").HasColumnType("datetime2(7)");
    }
}