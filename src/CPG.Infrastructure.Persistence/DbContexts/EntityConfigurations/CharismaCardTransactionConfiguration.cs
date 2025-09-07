using CPG.Domain.AggregateModels.TransactionAggregate;
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
		entity.Property(x => x.TrackId).HasMaxLength(255).HasColumnType("varchar").IsRequired();
		entity.Property(x => x.ProviderTrackId).HasMaxLength(255).HasColumnType("nvarchar").IsRequired();
		entity.Property(x => x.ReferenceNumber).HasMaxLength(255).HasColumnType("varchar");
		entity.Property(x => x.Status).HasColumnType("tinyint").IsRequired();
		entity.Property(x => x.SourceIban).HasColumnName("SourceIban").HasMaxLength(26).HasColumnType("varchar");
	}
}
