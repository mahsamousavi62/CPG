using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;

public class PaymentRequestMethodIpgTypeConfiguration : IEntityTypeConfiguration<PaymentRequestMethodIpgType>
{
    public void Configure(EntityTypeBuilder<PaymentRequestMethodIpgType> entity)
    {
        entity.ToTable("PaymentRequestMethodIpgType");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).UseIdentityColumn();
        entity.Property(x => x.PaymentRequestMethodId).HasColumnType("bigint").IsRequired();
        entity.Property(x => x.IpgTypeId).HasColumnType("bigint").IsRequired();

        entity.HasOne(x => x.PaymentRequestMethod)
            .WithMany(x => x.PaymentRequestMethodIpgTypes)
            .HasForeignKey(x => x.PaymentRequestMethodId)
            .OnDelete(DeleteBehavior.NoAction);

        entity.HasOne(x => x.IPGType)
            .WithMany(x => x.PaymentRequestMethodIpgTypes)
            .HasForeignKey(x => x.IpgTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
