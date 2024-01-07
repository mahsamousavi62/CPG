using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations;
public class PaymentRequestConfiguration : IEntityTypeConfiguration<PaymentRequest>
{
    public void Configure(EntityTypeBuilder<PaymentRequest> entity)
    {
        entity.ToTable("PaymentRequest");
        entity.HasKey(x => x.Id);

        entity.Ignore(x => x.DomainEvents);
        entity.Property(x => x.Id).HasColumnName("Id").UseIdentityColumn();
        entity.Property(x => x.CompanyId).HasColumnName("CompanyId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.ApplicationId).HasColumnName("ApplicationId").HasColumnType("bigint").IsRequired();
        entity.Property(x => x.DestinationIban).HasColumnName("DestinationIban").HasMaxLength(26).HasColumnType("nvarchar").IsRequired();
        entity.Property(x => x.NationalCode).HasColumnName("NationalCode").HasMaxLength(10).HasColumnType("varchar").IsRequired();
        entity.Property(x => x.Description).HasColumnName("Description").HasMaxLength(1000).HasColumnType("nvarchar");
        entity.Property(x => x.Amount).HasColumnName("Amount").HasColumnType("numeric").HasPrecision(18, 0).IsRequired();
        entity.Property(x => x.CallBackUrl).HasColumnName("CallBackUrl").HasMaxLength(2048).HasColumnType("varchar").IsRequired();
        entity.Property(x => x.PaymentCode).HasColumnName("PaymentCode").HasMaxLength(1000).HasColumnType("varchar").IsRequired();
        entity.Property(x => x.TrackerId).HasColumnName("TrackerId").HasMaxLength(255).HasColumnType("varchar").IsRequired();
        entity.Property(x => x.Status).HasColumnName("Status").HasColumnType("tinyint").IsRequired();
        entity.Property(x => x.IsUsed).HasColumnName("IsUsed").HasColumnType("bit").IsRequired();
        entity.Property(x => x.VerificationDateTime).HasColumnName("VerificationDateTime").HasColumnType("datetime2(7)");
        entity.Property(x => x.UrlExpirationDateTime).HasColumnName("UrlExpirationDateTime").HasColumnType("datetime2(7)").IsRequired();
    }

}
