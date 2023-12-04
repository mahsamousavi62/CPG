using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class CompanyIPGDepositConfiguration : IEntityTypeConfiguration<CompanyIPGDeposit>
    {
        public void Configure(EntityTypeBuilder<CompanyIPGDeposit> builder)
        {
            throw new NotImplementedException();
        }
    }
}
