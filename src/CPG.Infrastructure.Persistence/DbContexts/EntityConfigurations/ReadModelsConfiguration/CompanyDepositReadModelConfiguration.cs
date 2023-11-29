using CPG.Domain.SeedWork;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations.ReadModelsConfiguration
{
    public class CompanyDepositReadModelConfiguration : IEntityTypeConfiguration<CompanyDepositReadModel>
    {
        public void Configure(EntityTypeBuilder<CompanyDepositReadModel> readModel)
        {

            readModel.HasKey(x => x.Id);
            readModel.ToTable("CompanyDeposit");

            readModel.Property(x => x.Id).HasColumnName("Id");
            readModel.Property(x => x.Name).HasColumnName("Name");
            readModel.Property(x => x.Iban).HasColumnName("Iban");
            readModel.Property(x => x.BankId).HasColumnName("BankId");
            readModel.Property(x => x.AccountNumber).HasColumnName("AccountNumber");
            readModel.Property(x => x.CompanyId).HasColumnName("CompanyId");
            readModel.Property(x => x.IsActive);
            readModel.Property(x => x.ModificationDate);
            readModel.Property(x => x.CreationDate);

        }
    }
}
