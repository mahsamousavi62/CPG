using CPG.Domain.AggregateModels.TransactionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> entity)
    {
        entity.ToTable("Transaction");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.PaymentRquestId).HasColumnName("PaymentRquestId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.ReferenceTransactionId).HasColumnName("ReferenceTransactionId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.TransactionMethodType).HasColumnName("TransactionMethodType").HasColumnType("smallint").IsRequired();
        entity.Property(x => x.CompanyId).HasColumnName("CompanyId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.DestinationDepositId).HasColumnName("DestinationDepositId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.Amount).HasColumnName("Amount").HasColumnType("numeric").HasPrecision(18, 0).IsRequired();
        entity.Property(x => x.ApplicationId).HasColumnName("ApplicationId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.PredictedSettlementDateTime).HasColumnName("PredictedSettlementDateTime").HasColumnType("datetime2(7)");
        entity.Property(x => x.Status).HasColumnName("Status").HasColumnType("smallint").IsRequired();

       // entity.HasOne(p => p.PaymentRequest).WithOne(t => t.Transaction).HasForeignKey<Transaction>(b => b.PaymentRquestId);
        //entity.HasOne(t => t.IPGTransaction).WithOne(t => t.Transaction).HasForeignKey<Transaction>(t => t.ReferenceTransactionId);
    }
}
