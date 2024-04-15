using CPG.Domain.AggregateModels.TransactionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class TransactionReadModelConfiguration : IEntityTypeConfiguration<TransactionReadModel>
{
    public void Configure(EntityTypeBuilder<TransactionReadModel> entity)
    {
        entity.ToTable("Transaction");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id).HasColumnName("Id");
        entity.Property(x => x.PaymentRquestId).HasColumnName("PaymentRquestId");
        entity.Property(x => x.IPGTransactionId).HasColumnName("IPGTransactionId");
        entity.Property(x => x.DirectDebitTransactionId).HasColumnName("DirectDebitTransactionId");
        entity.Property(x => x.TransactionMethodType).HasColumnName("TransactionMethodType");
        entity.Property(x => x.CompanyId).HasColumnName("CompanyId");
        entity.Property(x => x.DestinationDepositId).HasColumnName("DestinationDepositId");
        entity.Property(x => x.Amount).HasColumnName("Amount");
        entity.Property(x => x.ApplicationId).HasColumnName("ApplicationId");
        entity.Property(x => x.PredictedSettlementDateTime).HasColumnName("PredictedSettlementDateTime");
        entity.Property(x => x.Status).HasColumnName("Status");

        entity.HasOne(p => p.PaymentRequest).WithOne(t => t.Transaction).HasForeignKey<TransactionReadModel>(b => b.PaymentRquestId);
        entity.HasOne(t => t.IPGTransaction).WithOne(t => t.Transaction).HasForeignKey<TransactionReadModel>(t => t.IPGTransactionId);
        entity.HasOne(t => t.DirectDebitTransaction).WithOne(t => t.Transaction).HasForeignKey<TransactionReadModel>(t => t.DirectDebitTransactionId);
        entity.HasOne(t => t.CharismaCardTransaction).WithOne(t => t.Transaction).HasForeignKey<TransactionReadModel>(t => t.CharismaCardTransactionId);
        entity.HasOne(t => t.PaymentReceiptTransaction).WithOne(t => t.Transaction).HasForeignKey<TransactionReadModel>(t => t.PaymentReceiptTransactionId);
        entity.HasOne(t => t.DestinationDeposit).WithMany(t => t.Transactions).HasForeignKey(t => t.DestinationDepositId);

    }
}
