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
        entity.Property(x => x.IPGTransactionId).HasColumnName("IPGTransactionId").HasColumnType("bigint");
        entity.Property(x => x.DirectDebitTransactionId).HasColumnName("DirectDebitTransactionId").HasColumnType("bigint");
        entity.Property(x => x.TransactionMethodType).HasColumnName("TransactionMethodType").HasColumnType("tinyint").IsRequired();
        entity.Property(x => x.CompanyId).HasColumnName("CompanyId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.DestinationDepositId).HasColumnName("DestinationDepositId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.Amount).HasColumnName("Amount").HasColumnType("numeric").HasPrecision(18, 0).IsRequired();
        entity.Property(x => x.ApplicationId).HasColumnName("ApplicationId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.PredictedSettlementDateTime).HasColumnName("PredictedSettlementDateTime").HasColumnType("datetime2");
        entity.Property(x => x.Status).HasColumnName("Status").HasColumnType("tinyint").IsRequired();

        entity.HasOne(p => p.PaymentRequest)
            .WithOne(t => t.Transaction)
            .HasForeignKey<Transaction>(b => b.PaymentRquestId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(t => t.IPGTransaction)
            .WithOne(t => t.Transaction)
            .HasForeignKey<Transaction>(t => t.IPGTransactionId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(t => t.DirectDebitTransaction)
            .WithOne(t => t.Transaction)
            .HasForeignKey<Transaction>(t => t.DirectDebitTransactionId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(t => t.PaymentReceiptTransaction)
            .WithOne(t => t.Transaction)
            .HasForeignKey<Transaction>(t => t.PaymentReceiptTransactionId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(t => t.DestinationDeposit)
            .WithMany(t => t.Transactions)
            .HasForeignKey(t => t.DestinationDepositId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
