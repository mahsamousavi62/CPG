using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class PaymentRequestMethodDepositConfiguration : IEntityTypeConfiguration<PaymentRequestMethodDeposit>
{
    public void Configure(EntityTypeBuilder<PaymentRequestMethodDeposit> entity)
    {
        entity.ToTable("PaymentRequestMethodDeposit");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).UseIdentityColumn();
        entity.Property(x => x.PaymentRequestMethodId).HasColumnType("bigint").IsRequired();
        entity.Property(x => x.CompanyDepositId).HasColumnType("bigint").IsRequired();

        entity.HasOne(x => x.PaymentRequestMethod)
            .WithMany(x => x.PaymentRequestMethodDeposits)
            .HasForeignKey(x => x.PaymentRequestMethodId);

        entity.HasOne(x => x.CompanyDeposit)
            .WithMany(x => x.PaymentRequestMethodDeposits)
            .HasForeignKey(x => x.CompanyDepositId);
    }
}