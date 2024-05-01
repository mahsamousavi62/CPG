using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class PaymentRequestMethodConfiguration : IEntityTypeConfiguration<PaymentRequestMethod>
{
    public void Configure(EntityTypeBuilder<PaymentRequestMethod> entity)
    {
        entity.ToTable("PaymentRequestMethod");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).UseIdentityColumn();
        entity.Property(x => x.PaymentRequestId).HasColumnType("bigint").IsRequired();
        entity.Property(x => x.PaymentMethodType).HasColumnType("tinyint").IsRequired();

        entity.HasOne(x => x.PaymentRequest)
            .WithMany(x => x.PaymentRequestMethods)
            .HasForeignKey(x => x.PaymentRequestId);
    }
}