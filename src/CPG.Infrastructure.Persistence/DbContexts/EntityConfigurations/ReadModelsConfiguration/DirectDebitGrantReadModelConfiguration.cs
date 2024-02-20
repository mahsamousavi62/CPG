using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration;

internal class DirectDebitGrantReadModelConfiguration : IEntityTypeConfiguration<DirectDebitGrantReadModel>
{
    public void Configure(EntityTypeBuilder<DirectDebitGrantReadModel> readModel)
    {
        readModel.ToTable("DirectDebitGrant");
        readModel.HasKey(x => x.Id);
        readModel.Property(x => x.Id);
        readModel.Property(x => x.UserId);
        readModel.Property(x => x.BankId);
        readModel.Property(x => x.AccountNumber);
        readModel.Property(x => x.PhoneNumber);
        readModel.Property(x => x.SuccessTransactionCountLimitPerMonth);
        readModel.Property(x => x.AmountLimitPerTransaction);
        readModel.Property(x => x.TrackId);
        readModel.Property(x => x.ExpirationDate);
        readModel.Property(x => x.RevokeDateTime);
        readModel.Property(x => x.ProviderId);
        readModel.Property(x => x.GrantToken);
        readModel.Property(x => x.AuthorizationId);
        readModel.Property(x => x.Status);
        readModel.Property(x => x.DurationPerMonth);
        readModel.Property(x => x.IsActive);
        readModel.Property(x => x.CreationDate);
        readModel.Property(x => x.ModificationDate);

        readModel.HasOne(x => x.Provider).WithMany(x => x.DirectDebitGrants).HasForeignKey(x => x.ProviderId);
    }
}