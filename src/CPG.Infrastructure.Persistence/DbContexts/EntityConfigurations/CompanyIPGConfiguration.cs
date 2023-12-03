using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CPG.Infrastructure.Persistence.DbContexts.EntityConfigurations
{
    public class CompanyIPGConfiguration : IEntityTypeConfiguration<CompanyIPG>
    {
        public void Configure(EntityTypeBuilder<CompanyIPG> builder)
        {
            throw new NotImplementedException();
        }
    }
}
